using rh.financeiro.Domain.Dto.Paginacao;
using rh.financeiro.Domain.Dto.Request.ContasFinanceiras.BuscarContasFinanceiras;
using rh.financeiro.Domain.Dto.Request.ContasFinanceiras.CriarContaFinanceira;
using rh.financeiro.Domain.Dto.Request.ContasFinanceiras.EditarContaFinanceira;
using rh.financeiro.Domain.Dto.Response.ContasFinanceiras;
using rh.financeiro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Interfaces.Service.ContaFinanceiras
{
    public interface IContasFinanceirasService
    {
        Task<ContaFinanceira?> CriarContaFinanceira(CriarContaFinanceiraRequest request, string UsuarioId);
        Task<PaginacaoResponse<BuscarContasFinanceirasResponse>> BuscarContasFinanceiras(BuscarContasFinanceirasRequest request, string UsuarioId);
        Task<ContaFinanceira?> BuscarContaFinanceiraPorId(string id, string UsuarioId);
        Task<ContaFinanceira?> EditarContaFinanceiraPorId(EditarContaFinanceiraRequest request, string id, string UsuarioId);
        Task<bool> DesativarOuReativarContaFinanceiraPorId(string id,string Acao, string UsuarioId);
    }
}
