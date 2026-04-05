using rh.financeiro.Domain.Dto.Paginacao;
using rh.financeiro.Domain.Dto.Request.CategoriaFinanceira.BuscarCategoriasFinanceiras;
using rh.financeiro.Domain.Dto.Request.CategoriaFinanceira.CriarCategoriaFinanceira;
using rh.financeiro.Domain.Dto.Request.CategoriaFinanceira.EditarCategoriaFinanceira;
using rh.financeiro.Domain.Dto.Response.CategoriaFinanceira;
using rh.financeiro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Interfaces.Service.CategoriaFinanceiras
{
    public interface ICategoriaFinanceiraService
    {
        Task<CategoriaFinanceira?> CriarCategoriaFinanceira(CriarCategoriaFinanceiraRequest request, string UsuarioId);
        Task<PaginacaoResponse<BuscarCategoriasFinanceirasResponse>> BuscarCategoriasFinanceiras(BuscarCategoriasFinanceirasRequest request, string UsuarioId);
        Task<CategoriaFinanceira?> BuscarCategoriaFinanceiraPorId(string id, string UsuarioId);
        Task<CategoriaFinanceira?> EditarCategoriaFinanceiraPorId(EditarCategoriaFinanceiraRequest request, string id, string UsuarioId);
        Task<bool> DesativarOuReativarCategoriaFinaceiraPorId(string id, string Acao, string UsuarioId);
    }
}
