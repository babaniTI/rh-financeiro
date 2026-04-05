using Microsoft.EntityFrameworkCore;
using rh.financeiro.Data.Context.Default;
using rh.financeiro.Domain.Dto.Paginacao;
using rh.financeiro.Domain.Dto.Request.Titulos.BuscarTitulos;
using rh.financeiro.Domain.Dto.Response.DocumentoFiscal.BuscarDocumentosFiscais;
using rh.financeiro.Domain.Dto.Response.Titulos.BuscarTitulos;
using rh.financeiro.Domain.Entities;
using rh.financeiro.Domain.Enums;
using rh.financeiro.Domain.Interfaces.Service.Titulo;
using rh.financeiro.Domain.Interfaces.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Services.Services.Titulos
{
    public class TituloService : ITituloService
    {
        #region Fields
        private readonly IUnitOfWork<DefaultContext> _unitOfWork;
        #endregion

        #region CONS
        public TituloService(IUnitOfWork<DefaultContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Public Methods
        public async Task<PaginacaoResponse<BuscarTitulosResponse>> BuscarTitulos(BuscarTitulosRequest request, string UsuarioId)
        {
            #region Fields
            var EmpresaId = await BuscarEmpresaIdPorUsarioId(UsuarioId);
            var repoDocumentoFiscal = _unitOfWork.Repository<DocumentoFiscal>().QueryableObject();
            var repoTitulo = _unitOfWork.Repository<Titulo>().QueryableObject();
            var repoParcela = _unitOfWork.Repository<Parcela>().QueryableObject();
            var repoParticipante = _unitOfWork.Repository<Participante>().QueryableObject();
            #endregion

            #region Query
            var query =
                from df in repoDocumentoFiscal
                join ti in repoTitulo
                    on df.Id equals ti.DocumentoId
                join pt in repoParticipante
                    on df.ParticipanteId equals pt.Id
                where df.EmpresaId == EmpresaId
                select new
                {
                    Documento = df,
                    Partipante = pt,
                    Titulo = ti
                };
            #endregion

            #region Logic
            try
            {

                if (!string.IsNullOrEmpty(request.tipo))
                    query = query.Where(x => x.Titulo.Tipo.ToString() == request.tipo);

                if (!string.IsNullOrEmpty(request.status))
                    query = query.Where(x => x.Titulo.Status.ToString() == request.status);

                #region Filtro por Data de Emissão (ISO 8601)
                DateTimeOffset? dataInicio = string.IsNullOrEmpty(request.dataEmissaoInicio)
                    ? null
                    : DateTimeOffset.Parse(request.dataEmissaoInicio);

                DateTimeOffset? dataFim = string.IsNullOrEmpty(request.dataEmissaoFim)
                     ? null
                     : DateTimeOffset.Parse(request.dataEmissaoFim)
                        .Date
                        .AddDays(1)
                        .AddTicks(-1);

                if (dataInicio.HasValue || dataFim.HasValue)
                {
                    query = query.Where(x =>
                        (!dataInicio.HasValue || x.Documento.DataEmissao >= dataInicio.Value.DateTime) &&
                        (!dataFim.HasValue || x.Documento.DataEmissao <= dataFim.Value.DateTime)
                    );
                }
                #endregion

                if (!string.IsNullOrEmpty(request.search))
                {
                    var search = request.search.Trim();
                    query = query.Where(x =>
                        x.Documento.Numero.ToString().Equals(search)
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
                        Documento = x.Documento,
                        Partipante = x.Partipante,
                        Titulo = x.Titulo,
                        Parcelas = repoParcela
                        .Where(p => p.TituloId == x.Titulo.Id)
                        .ToList()
                    })
                    .ToListAsync();

                var result = data.Select(x => new BuscarTitulosResponse
                {
                    Id = x.Titulo.Id,
                    EmpresaId = EmpresaId,
                    Tipo = x.Titulo.Tipo,
                    Numero = x.Documento.Numero,
                    NomeParticipante = x.Partipante.Nome,
                    DataEmissao = x.Documento.DataEmissao,
                    ValorTotal = x.Documento.ValorTotal,
                    ValorPago = x.Parcelas.Where(x => x.Status == StatusParcela.Pago).Sum(x => x.Valor),
                    ValorAtual = x.Documento.ValorTotal - x.Parcelas.Where(x => x.Status == StatusParcela.Pago).Sum(x => x.Valor), // Total - Pago
                    Parcelas = x.Parcelas.ToList(),
                    Status = x.Titulo.Status,
                    CreatedAt = x.Titulo.CreatedAt,
                }).ToList();

                return new PaginacaoResponse<BuscarTitulosResponse>
                {
                    total = total,
                    page = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling((double)total / pageSize),
                    data = result
                };
            }
            catch(Exception ex)
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
