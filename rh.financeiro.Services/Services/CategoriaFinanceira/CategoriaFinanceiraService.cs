using Microsoft.EntityFrameworkCore;
using rh.financeiro.Data.Context.Default;
using rh.financeiro.Domain.Dto.Paginacao;
using rh.financeiro.Domain.Dto.Request.CategoriaFinanceira.BuscarCategoriasFinanceiras;
using rh.financeiro.Domain.Dto.Request.CategoriaFinanceira.CriarCategoriaFinanceira;
using rh.financeiro.Domain.Dto.Request.CategoriaFinanceira.EditarCategoriaFinanceira;
using rh.financeiro.Domain.Dto.Request.ContasFinanceiras.BuscarContasFinanceiras;
using rh.financeiro.Domain.Dto.Request.ContasFinanceiras.CriarContaFinanceira;
using rh.financeiro.Domain.Dto.Request.ContasFinanceiras.EditarContaFinanceira;
using rh.financeiro.Domain.Dto.Response.CategoriaFinanceira;
using rh.financeiro.Domain.Dto.Response.ContasFinanceiras;
using rh.financeiro.Domain.Entities;
using rh.financeiro.Domain.Enums;
using rh.financeiro.Domain.Interfaces.Service.CategoriaFinanceiras;
using rh.financeiro.Domain.Interfaces.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Services.Services.CategoriaFinanceiras
{
    public class CategoriaFinanceiraService : ICategoriaFinanceiraService
    {
        #region Fields
        private readonly IUnitOfWork<DefaultContext> _unitOfWork;
        #endregion

        #region CONS
        public CategoriaFinanceiraService(IUnitOfWork<DefaultContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion
        #region Public Methods

        public async Task<CategoriaFinanceira?> CriarCategoriaFinanceira(CriarCategoriaFinanceiraRequest request, string UsuarioId)
        {
            #region Fields
            var repoCategoriaFinanceira = _unitOfWork.Repository<CategoriaFinanceira>();
            var repoUsuarioEmpresa = _unitOfWork.Repository<UsuarioEmpresa>();
            Guid EmpresaId = await BuscarEmpresaIdPorUsarioId(UsuarioId);
            #endregion

            #region Logic
            try
            {
                // Definindo o tipo
                var tipo = request.tipo.ToUpper() switch
                {
                    "RECEITA" => TipoCategoriaFinanceira.Receita,
                    "DESPESA" => TipoCategoriaFinanceira.Despesa,
                    "TRANSFERENCIA" => TipoCategoriaFinanceira.Transferencia
                };


                bool jaExiste = await repoCategoriaFinanceira.QueryableObject()
                    .AnyAsync(x => x.Tipo == tipo && x.EmpresaId == EmpresaId);

                if (jaExiste)
                    return null;

                var NovaCategoriaFinanceira = new CategoriaFinanceira()
                {
                    EmpresaId = EmpresaId,
                    Tipo = request.tipo switch
                    {
                        "RECEITA" => TipoCategoriaFinanceira.Receita,
                        "DESPESA" => TipoCategoriaFinanceira.Despesa,
                        "TRANSFERENCIA" => TipoCategoriaFinanceira.Transferencia
                    },
                    Nome = request.descricao,
                    Codigo = request.codigo,
                    Cfop = request.Cfop,
                    Ativo = true
                };

                await repoCategoriaFinanceira.AddAsync(NovaCategoriaFinanceira);
                return NovaCategoriaFinanceira;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion
        }


        public async Task<PaginacaoResponse<BuscarCategoriasFinanceirasResponse>> BuscarCategoriasFinanceiras(BuscarCategoriasFinanceirasRequest request, string UsuarioId)
        {
            #region Fields
            var queryCategoriaFinanceira = _unitOfWork.Repository<CategoriaFinanceira>().QueryableObject();
            #endregion

            #region Logic
            try
            {
                // Buscando a EmpresaId pelo usuario autenticado
                Guid EmpresaId = await BuscarEmpresaIdPorUsarioId(UsuarioId);

                // Filtro por Empresa
                queryCategoriaFinanceira = queryCategoriaFinanceira
                    .Where(x => x.EmpresaId.ToString() == EmpresaId.ToString());

                // Filtro por tipo
                if (request.tipo != null)
                {
                    queryCategoriaFinanceira = queryCategoriaFinanceira
                        .Where(x => x.Tipo.ToString() == request.tipo);
                }

                // Filtrar por ativo
                queryCategoriaFinanceira = queryCategoriaFinanceira
                    .Where(x => x.Ativo == request.Ativo);

                var total = await queryCategoriaFinanceira.CountAsync();

                // Paginação
                var page = request.page <= 0 ? 1 : request.page;
                var pageSize = request.pageSize <= 0 ? 10 : request.pageSize;

                var data = await queryCategoriaFinanceira
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new BuscarCategoriasFinanceirasResponse()
                    {
                        id = x.Id.ToString(),
                        codigo = x.Codigo,
                        cfop = x.Cfop,
                        descricao = x.Nome,
                        tipo = x.Tipo.ToString(),
                        ativo = x.Ativo,
                    })
                    .ToListAsync();

                return new PaginacaoResponse<BuscarCategoriasFinanceirasResponse>
                {
                    total = total,
                    page = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling((double)total / pageSize),
                    data = data
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion

        }

        public async Task<CategoriaFinanceira?> BuscarCategoriaFinanceiraPorId(string id, string UsuarioId)
        {
            #region Fields
            #endregion

            #region Logic
            try
            {
                // Buscando a EmpresaId pelo usuario autenticado
                Guid EmpresaId = await BuscarEmpresaIdPorUsarioId(UsuarioId);

                return await _unitOfWork.Repository<CategoriaFinanceira>()
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

        public async Task<CategoriaFinanceira?> EditarCategoriaFinanceiraPorId(EditarCategoriaFinanceiraRequest request, string id, string UsuarioId)
        {
            #region Fields
            var repoCategoriaFinanceira = _unitOfWork.Repository<CategoriaFinanceira>();
            #endregion

            #region Logic
            try
            {
                // Buscando a EmpresaId pelo usuario autenticado
                Guid EmpresaId = await BuscarEmpresaIdPorUsarioId(UsuarioId);

                var categoriaFinanceira = await repoCategoriaFinanceira.QueryableObject()
                    .FirstOrDefaultAsync(x => x.Id.ToString() == id && x.EmpresaId.ToString() == EmpresaId.ToString());

                if (categoriaFinanceira == null)
                    return null;

                if (request.codigo != null)
                    categoriaFinanceira.Codigo = request.codigo;

                if (request.cfop != null)
                    categoriaFinanceira.Cfop = request.cfop;

                if (request.descricao != null)
                    categoriaFinanceira.Nome = request.descricao;

                if (request.tipo != null)
                {
                    categoriaFinanceira.Tipo = request.tipo.ToUpper() switch
                    {
                        "RECEITA" => TipoCategoriaFinanceira.Receita,
                        "DESPESA" => TipoCategoriaFinanceira.Despesa,
                        "TRANSFERENCIA" => TipoCategoriaFinanceira.Transferencia,
                    };
                }

                await repoCategoriaFinanceira.UpdateAsync(categoriaFinanceira);
                return categoriaFinanceira;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion
        }

        public async Task<bool> DesativarOuReativarCategoriaFinaceiraPorId(string id, string Acao, string UsuarioId)
        {
            #region Fields
            var repoCategoriaFinanceira = _unitOfWork.Repository<CategoriaFinanceira>();
            #endregion

            #region Logicd
            try
            {
                // Buscando a EmpresaId pelo usuario autenticado
                Guid EmpresaId = await BuscarEmpresaIdPorUsarioId(UsuarioId);

                var categoriaFinanceira = await repoCategoriaFinanceira.QueryableObject()
                    .FirstOrDefaultAsync(x => x.Id.ToString() == id && x.EmpresaId.ToString() == EmpresaId.ToString());

                if (categoriaFinanceira == null)
                    return false;

                // Fazendo o controle

                if (Acao.ToUpper() == "ATIVAR")
                    categoriaFinanceira.Ativo = true;

                if (Acao.ToUpper() == "DESATIVAR")
                    categoriaFinanceira.Ativo = false;

                await repoCategoriaFinanceira.UpdateAsync(categoriaFinanceira);

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

