using Microsoft.EntityFrameworkCore;
using ouroeprata.comprarapida.Data.UnitOfWorks;
using rh.financeiro.CrossCuting;
using rh.financeiro.Data.Context.Default;
using rh.financeiro.Domain.Common.DocumentosNfes;
using rh.financeiro.Domain.Dto.Paginacao;
using rh.financeiro.Domain.Dto.Request.DocumentoFiscal.BuscarDocumentosFiscais;
using rh.financeiro.Domain.Dto.Request.MovimentacaoBancaria.BuscarMovimentacoesBancarias;
using rh.financeiro.Domain.Dto.Response.DocumentoFiscal.BuscarDocumentosFiscais;
using rh.financeiro.Domain.Dto.Response.MovimentacaoBancaria.BuscarMovimentacoesBancarias;
using rh.financeiro.Domain.Entities;
using rh.financeiro.Domain.Enums;
using rh.financeiro.Domain.Interfaces.Service.MovimentacaoBancaria;
using rh.financeiro.Domain.Interfaces.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace rh.financeiro.Services.Services.MovimentacaoBancaria
{
    public class MovimentacaoBancariaService : IMovimentacaoBancariaService
    {
        #region Fields
        private readonly IUnitOfWork<DefaultContext> _unitOfWork;
        #endregion

        #region CONS
        public MovimentacaoBancariaService(IUnitOfWork<DefaultContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Public Methods
        public async Task<PaginacaoResponse<BuscarMovimentacoesBancariasResponse>> BuscarMovimentacoesBancarias(BuscarMovimentacoesBancariasRequest request, string UsuarioId)
        {
            #region Fields
            var EmpresaId = await BuscarEmpresaIdPorUsarioId(UsuarioId);
            var repoMovimentacaoBancaria = _unitOfWork.Repository<MovimentoBancario>().QueryableObject();
            var repoContaFinanceira = _unitOfWork.Repository<ContaFinanceira>().QueryableObject();
            #endregion

            #region Query
            var query =
                from mvb in repoMovimentacaoBancaria
                join cf in repoContaFinanceira
                    on mvb.ContaId equals cf.Id
                select new
                {
                    Movimentacao = mvb,
                    ContaFinanceira = cf
                };
            #endregion

            #region Logic
            try
            {

                if (!string.IsNullOrEmpty(request.tipo))
                {
                    var tipoEnum = Enum.Parse<TipoMovimento>(request.tipo, true);
                    query = query.Where(x => x.Movimentacao.Tipo == tipoEnum);
                }

                if (!string.IsNullOrEmpty(request.status))
                {
                    var statusEnum = Enum.Parse<StatusMovimento>(request.status, true);
                    query = query.Where(x => x.Movimentacao.Status == statusEnum);

                }

                #region Filtro por Data de Emissão (ISO 8601)
                DateTimeOffset? dataInicio = string.IsNullOrEmpty(request.dataInicio)
                    ? null
                    : DateTimeOffset.Parse(request.dataInicio);

                DateTimeOffset? dataFim = string.IsNullOrEmpty(request.dataFim)
                     ? null
                     : DateTimeOffset.Parse(request.dataFim)
                        .Date
                        .AddDays(1)
                        .AddTicks(-1);

                if (dataInicio.HasValue || dataFim.HasValue)
                {
                    query = query.Where(x =>
                        (!dataInicio.HasValue || x.Movimentacao.Data >= dataInicio.Value.DateTime) &&
                        (!dataFim.HasValue || x.Movimentacao.Data <= dataFim.Value.DateTime)
                    );
                }
                #endregion

                if (!string.IsNullOrEmpty(request.search))
                {
                    var search = request.search.Trim();
                    query = query.Where(x =>
                        (x.Movimentacao.Descricao == search)
                        ||
                        x.Movimentacao.hashUnico == search
                    );
                }

                var total = await query.CountAsync();

                // Paginação
                var page = request.page <= 0 ? 1 : request.page;
                var pageSize = request.pageSize <= 0 ? 10 : request.pageSize;

                var data = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new
                    {
                        Movimentacao = x.Movimentacao,
                        ContaFinanceira = x.ContaFinanceira
                    })
                    .ToListAsync();

                var result = data.Select(x => new BuscarMovimentacoesBancariasResponse
                {
                    Id = x.Movimentacao.Id.ToString(),
                    Data = x.Movimentacao.Data!,
                    Descricao = x.Movimentacao.Descricao,
                    ValorBruto = x.Movimentacao.ValorBruto,
                    Tarifa = x.Movimentacao.Tarifa,
                    ValorLiquido = x.Movimentacao.ValorLiquido,
                    Status = x.Movimentacao.Status,
                    Conta = x.ContaFinanceira.Descricao
                }).ToList();

                return new PaginacaoResponse<BuscarMovimentacoesBancariasResponse>
                {
                    total = total,
                    page = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling((double)total / pageSize),
                    data = result
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            #endregion

        }

        public async Task<bool> ImportarOfx(string usuarioId, string ofx)
        {
            Guid empresaId = await BuscarEmpresaIdPorUsarioId(usuarioId);

            try
            {
                var doc = ConverterOfxParaXml(ofx);

                var contaNode = doc.Descendants("BANKACCTFROM").FirstOrDefault();

                var banco = contaNode?.Element("BANKID")?.Value;
                var branchId = contaNode?.Element("BRANCHID")?.Value;
                var acctId = contaNode?.Element("ACCTID")?.Value;

                // 🔥 quebra tudo
                var (agencia, agenciaDigito) = ExtrairAgenciaEDigito(branchId);
                var (conta, contaDigito) = ExtrairContaEDigito(acctId);

                // 🔍 busca real
                var contaId = await BuscarContaFinanceira(
                    banco,
                    agencia,
                    agenciaDigito,
                    conta,
                    contaDigito
                );

                if (contaId == Guid.Empty)
                    return false;

                var transacoes = doc.Descendants("STMTTRN");

                foreach (var trn in transacoes)
                {
                    var dataStr = trn.Element("DTPOSTED")?.Value?.Trim();
                    var valorStr = trn.Element("TRNAMT")?.Value?.Trim();

                    if (string.IsNullOrEmpty(dataStr) || string.IsNullOrEmpty(valorStr))
                        continue;

                    var data = ParseDataOfx(dataStr);
                    var valor = decimal.Parse(valorStr, CultureInfo.InvariantCulture);

                    var tipo = valor < 0 ? TipoMovimento.Debito : TipoMovimento.Credito;

                    var fitId = trn.Element("FITID")?.Value?.Trim();
                    var descricao = trn.Element("MEMO")?.Value?.Trim();

                    var hash = GerarHash(contaId, data, valor, fitId);

                    var movimento = new MovimentoBancario
                    {
                        Id = Guid.NewGuid(),
                        ContaId = contaId,
                        Data = data.Date,
                        Tipo = tipo,
                        ValorBruto = valor,
                        ValorLiquido = valor,
                        Tarifa = 0,
                        Descricao = descricao,
                        IdentificadorExterno = fitId,
                        hashUnico = hash,
                        Conciliado = false,
                        Status = StatusMovimento.PENDENTE,
                    };

                    if (!await ExisteHash(hash))
                    {
                        await _unitOfWork.Repository<MovimentoBancario>().AddAsync(movimento);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        #endregion

        #region Private Methods
        private async Task<Guid> BuscarEmpresaIdPorUsarioId(string UsuarioId)
        {
            #region Fields
            #endregion

            #region Logic
            try
            {
                // Buscando a EmpresaId pelo usuario autenticado
                return await _unitOfWork.Repository<UsuarioEmpresa>()
                    .QueryableObject()
                    .Where(x => x.UsuarioId.ToString() == UsuarioId)
                    .Select(x => x.EmpresaId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion
        }

        private (string conta, string digito) ExtrairContaEDigito(string acctId)
        {
            if (string.IsNullOrEmpty(acctId))
                return (null, null);

            acctId = acctId.Trim();

            // Caso tenha hífen → padrão mais confiável
            if (acctId.Contains("-"))
            {
                var partes = acctId.Split('-');
                return (partes[0], partes.Length > 1 ? partes[1] : null);
            }

            // Caso só número → assume último dígito como verificador
            if (acctId.Length > 1)
            {
                var conta = acctId.Substring(0, acctId.Length - 1);
                var digito = acctId.Substring(acctId.Length - 1, 1);

                return (conta, digito);
            }

            return (acctId, null);
        }

        private (string agencia, string digito) ExtrairAgenciaEDigito(string branchId)
        {
            if (string.IsNullOrEmpty(branchId))
                return (null, null);

            branchId = branchId.Trim();

            if (branchId.Contains("-"))
            {
                var partes = branchId.Split('-');
                return (partes[0], partes.Length > 1 ? partes[1] : null);
            }

            return (branchId, null);
        }

        private async Task<Guid> BuscarContaFinanceira(
            string banco,
            string agencia,
            string agenciaDigito,
            string conta,
            string contaDigito)
        {
            var query = _unitOfWork.Repository<ContaFinanceira>().QueryableObject();

            // Match mais completo primeiro
            var contaEncontrada = await query.FirstOrDefaultAsync(x =>
                x.Banco == banco &&
                x.Agencia == $"{agencia}-{agenciaDigito}" &&
                x.Conta == $"{conta}-{contaDigito}"
            );

            if (contaEncontrada != null)
                return contaEncontrada.Id;

            contaEncontrada = query
                .Where(x => x.Banco == banco)
                .AsEnumerable()
                .FirstOrDefault(x =>
                    x.Conta.Split("-")[0] == conta &&
                    x.Agencia.Split("-")[0] == agencia
                );

            if (contaEncontrada != null)
                return contaEncontrada.Id;

            // Fallback só conta (caso Safra)
            contaEncontrada = await query.FirstOrDefaultAsync(x =>
                x.Banco == banco &&
                x.Conta == conta
            );

            return contaEncontrada?.Id ?? Guid.Empty;
        }

        private XDocument ConverterOfxParaXml(string ofx)
        {
            // Remove header (OFX antigo)
            var bodyIndex = ofx.IndexOf("<OFX>");
            if (bodyIndex > 0)
                ofx = ofx.Substring(bodyIndex);

            // Fecha tags abertas automaticamente
            ofx = FecharTags(ofx);

            return XDocument.Parse(ofx);
        }

        private string FecharTags(string input)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"<(\w+)>([^<\r\n]+)");
            return regex.Replace(input, "<$1>$2</$1>");
        }

        private DateTime ParseDataOfx(string data)
        {
            // formato: 20240401120000
            var limpa = data.Substring(0, 14);
            return DateTime.ParseExact(limpa, "yyyyMMddHHmmss", null);
        }

        private string GerarHash(Guid contaId, DateTime data, decimal valor, string fitId)
        {
            var raw = $"{contaId}-{data:yyyyMMddHHmmss}-{valor}-{fitId}";

            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(raw);
            var hash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

        private async Task<bool> ExisteHash(string hash)
        {
            #region Fields
            #endregion

            #region Logic
            try
            {
                // Buscando a EmpresaId pelo usuario autenticado
                return await _unitOfWork.Repository<MovimentoBancario>()
                    .QueryableObject()
                    .AnyAsync(x => x.hashUnico.ToString() == hash);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion
        }
        #endregion
    }
}
