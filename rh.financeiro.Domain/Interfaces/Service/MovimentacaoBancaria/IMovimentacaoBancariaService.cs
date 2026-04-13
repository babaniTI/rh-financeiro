using rh.financeiro.Domain.Dto.Paginacao;
using rh.financeiro.Domain.Dto.Request.MovimentacaoBancaria.BuscarMovimentacoesBancarias;
using rh.financeiro.Domain.Dto.Response.MovimentacaoBancaria.BuscarMovimentacoesBancarias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Interfaces.Service.MovimentacaoBancaria
{
    public interface IMovimentacaoBancariaService
    {
        Task<PaginacaoResponse<BuscarMovimentacoesBancariasResponse>> BuscarMovimentacoesBancarias(BuscarMovimentacoesBancariasRequest request, string UsuarioId);
        Task<bool> ImportarOfx(string usuarioId, string ofx);
    }
}
