using Microsoft.EntityFrameworkCore;
using rh.financeiro.Data.Context.Default;
using rh.financeiro.Domain.Dto.Paginacao;
using rh.financeiro.Domain.Dto.Request.ContasFinanceiras.BuscarContasFinanceiras;
using rh.financeiro.Domain.Dto.Request.ContasFinanceiras.CriarContaFinanceira;
using rh.financeiro.Domain.Dto.Request.ContasFinanceiras.EditarContaFinanceira;
using rh.financeiro.Domain.Dto.Request.Participantes.CriarParticipante;
using rh.financeiro.Domain.Dto.Request.Participantes.EditarParticipante;
using rh.financeiro.Domain.Dto.Response.ContasFinanceiras;
using rh.financeiro.Domain.Dto.Response.Participantes.BuscarParticipantes;
using rh.financeiro.Domain.Entities;
using rh.financeiro.Domain.Enums;
using rh.financeiro.Domain.Interfaces.Service.ContaFinanceiras;
using rh.financeiro.Domain.Interfaces.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Services.Services.ContasFinanceiras
{
    public class ContasFinanceirasService : IContasFinanceirasService
    {
        #region Fields
        private readonly IUnitOfWork<DefaultContext> _unitOfWork;
        #endregion

        #region CONS
        public ContasFinanceirasService(IUnitOfWork<DefaultContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Public Methods

        public async Task<ContaFinanceira?> CriarContaFinanceira(CriarContaFinanceiraRequest request, string UsuarioId)
        {
            #region Fields
            var repoContaFinanceira = _unitOfWork.Repository<ContaFinanceira>();
            var repoUsuarioEmpresa = _unitOfWork.Repository<UsuarioEmpresa>();
            Guid EmpresaId = await BuscarEmpresaIdPorUsarioId(UsuarioId);
            string agencia = $"{request.agencia}-{request.agenciaDigito}";
            string conta = $"{request.conta}-{request.contaDigito}";
            #endregion

            #region Logic
            try
            {
                bool jaExiste = await repoContaFinanceira.QueryableObject()
                    .AnyAsync(x => x.Agencia == agencia && x.Conta == conta && x.EmpresaId == EmpresaId);

                if (jaExiste)
                    return null;

                var NovaContaFinanceira = new ContaFinanceira()
                {
                    EmpresaId = EmpresaId,
                    Tipo = request.tipo switch
                    {
                        "CORRENTE" => TipoConta.Corrente,
                        "POUPANCA" => TipoConta.Poupanca,
                        "CAIXA" => TipoConta.Caixa,
                        "INVESTIMENTO" => TipoConta.Investimento,
                        "CARTAO" => TipoConta.Cartao
                    },
                    Banco = request.banco,
                    Agencia = agencia,
                    Conta = conta,
                    Descricao = request.descricao,
                    SaldoInicial = (decimal)request.saldoInicial,
                    Ativo = true
                };

                await repoContaFinanceira.AddAsync(NovaContaFinanceira);
                return NovaContaFinanceira;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion
        }


        public async Task<PaginacaoResponse<BuscarContasFinanceirasResponse>> BuscarContasFinanceiras(BuscarContasFinanceirasRequest request, string UsuarioId)
        {
            #region Fields
            var queryContaFinanceira = _unitOfWork.Repository<ContaFinanceira>().QueryableObject();
            var repoMovimentacao = _unitOfWork.Repository<MovimentoBancario>().QueryableObject();
            #endregion

            #region Logic
            try
            {
                // Buscando a EmpresaId pelo usuario autenticado
                Guid EmpresaId = await BuscarEmpresaIdPorUsarioId(UsuarioId);

                // Filtro por Empresa
                queryContaFinanceira = queryContaFinanceira
                    .Where(x => x.EmpresaId.ToString() == EmpresaId.ToString());

                // Filtro por tipo
                if (request.tipo != null)
                {
                    queryContaFinanceira = queryContaFinanceira
                        .Where(x => x.Tipo.ToString() == request.tipo);
                }

                // Filtrar por ativo
                queryContaFinanceira = queryContaFinanceira
                    .Where(x => x.Ativo == request.Ativo);

                var total = await queryContaFinanceira.CountAsync();

                // Paginação
                var page = request.page <= 0 ? 1 : request.page;
                var pageSize = request.pageSize <= 0 ? 10 : request.pageSize;


                var data = await queryContaFinanceira
                    .OrderByDescending(x => x.UpdatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(); // 🔥 executa no banco primeiro

                var movimentacoes = await repoMovimentacao
                    .GroupBy(m => m.ContaId)
                    .Select(g => new
                    {
                        ContaId = g.Key,
                        SaldoTotal = g.Sum(x => x.ValorBruto),
                        UltimaMovimentacao = g.Max(x => x.createdAt)
                    })
                    .ToListAsync();


                var result = data.Select(x =>
                {
                    var agenciaSplit = x.Agencia?.Split('-');
                    var contaSplit = x.Conta?.Split('-');

                    var mov = movimentacoes.FirstOrDefault(m => m.ContaId == x.Id);

                    return new BuscarContasFinanceirasResponse
                    {
                        id = x.Id.ToString(),
                        descricao = x.Descricao,
                        tipo = x.Tipo.ToString(),
                        banco = x.Banco,
                        agencia = agenciaSplit?.Length > 0 ? agenciaSplit[0] : null,
                        agenciaDigito = agenciaSplit?.Length > 1 ? agenciaSplit[1] : null,
                        conta = contaSplit?.Length > 0 ? contaSplit[0] : null,
                        contaDigito = contaSplit?.Length > 1 ? contaSplit[1] : null,
                        saldoAtual = mov.SaldoTotal + x.SaldoInicial,
                        dataUltimoMovimento = mov.UltimaMovimentacao.ToString("O"),
                        Ativo = x.Ativo,
                        createdAt = x.CreatedAt.ToString("O"),
                        updatedAt = x.UpdatedAt.ToString("O")
                    };
                }).ToList();

                return new PaginacaoResponse<BuscarContasFinanceirasResponse>
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

        public async Task<ContaFinanceira?> BuscarContaFinanceiraPorId(string id, string UsuarioId)
        {
            #region Fields
            #endregion

            #region Logic
            try
            {
                // Buscando a EmpresaId pelo usuario autenticado
                Guid EmpresaId = await BuscarEmpresaIdPorUsarioId(UsuarioId);

                return await _unitOfWork.Repository<ContaFinanceira>()
                    .QueryableObject()
                    .Where(x => x.Id.ToString() == id)
                    .Where(x => x.EmpresaId.ToString() == EmpresaId.ToString())
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion

        }

        public async Task<ContaFinanceira?> EditarContaFinanceiraPorId(EditarContaFinanceiraRequest request, string id, string UsuarioId)
        {
            #region Fields
            var repoContaFinanceira = _unitOfWork.Repository<ContaFinanceira>();
            #endregion

            #region Logic
            try
            {
                // Buscando a EmpresaId pelo usuario autenticado
                Guid EmpresaId = await BuscarEmpresaIdPorUsarioId(UsuarioId);

                var contaFinanceira = await repoContaFinanceira.QueryableObject()
                    .FirstOrDefaultAsync(x => x.Id.ToString() == id && x.EmpresaId.ToString() == EmpresaId.ToString());

                if (contaFinanceira == null)
                    return null;

                if (request.banco != null)
                    contaFinanceira.Banco = request.banco;

                if (request.agencia != null && request.agenciaDigito != null)
                {
                    string agencia = $"{request.agencia}-{request.agenciaDigito}";
                    contaFinanceira.Agencia = agencia;
                }

                if (request.conta != null && request.contaDigito != null)
                {
                    string conta = $"{request.conta}-{request.contaDigito}";
                    contaFinanceira.Conta = conta;
                }

                if (request.tipo != null)
                {
                    contaFinanceira.Tipo = request.tipo.ToUpper() switch
                    {
                        "CORRENTE" => TipoConta.Corrente,
                        "POUPANCA" => TipoConta.Poupanca,
                        "CAIXA" => TipoConta.Caixa,
                        "INVESTIMENTO" => TipoConta.Investimento,
                        "CARTAO" => TipoConta.Cartao
                    };
                }

                await repoContaFinanceira.UpdateAsync(contaFinanceira);
                return contaFinanceira;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion
        }

        public async Task<bool> DesativarOuReativarContaFinanceiraPorId(string id, string Acao, string UsuarioId)
        {
            #region Fields
            var repoContaFinanceira = _unitOfWork.Repository<ContaFinanceira>();
            #endregion

            #region Logic
            try
            {
                // Buscando a EmpresaId pelo usuario autenticado
                Guid EmpresaId = await BuscarEmpresaIdPorUsarioId(UsuarioId);

                var contaFinanceira = await repoContaFinanceira.QueryableObject()
                    .FirstOrDefaultAsync(x => x.Id.ToString() == id && x.EmpresaId.ToString() == EmpresaId.ToString());

                if (contaFinanceira == null)
                    return false;

                // Fazendo o controle

                if(Acao.ToUpper() == "ATIVAR")
                    contaFinanceira.Ativo = true;

                if (Acao.ToUpper() == "DESATIVAR")
                    contaFinanceira.Ativo = false;

                await repoContaFinanceira.UpdateAsync(contaFinanceira);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion
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
        #endregion
    }
}
