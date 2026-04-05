using rh.financeiro.Domain.Dto.Paginacao;
using rh.financeiro.Domain.Dto.Request.Titulos.BuscarTitulos;
using rh.financeiro.Domain.Dto.Response.Titulos.BuscarTitulos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Interfaces.Service.Titulo
{
    public interface ITituloService
    {
        Task<PaginacaoResponse<BuscarTitulosResponse>> BuscarTitulos(BuscarTitulosRequest request, string UsuarioId);
    }
}
