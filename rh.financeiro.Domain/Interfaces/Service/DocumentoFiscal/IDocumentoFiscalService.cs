using rh.financeiro.Domain.Dto.Paginacao;
using rh.financeiro.Domain.Dto.Request.DocumentoFiscal.BuscarDocumentosFiscais;
using rh.financeiro.Domain.Dto.Response.DocumentoFiscal.BuscarDocumentosFiscais;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Interfaces.Service.DocumentoFiscal
{
    public interface IDocumentoFiscalService
    {
        Task<PaginacaoResponse<BuscarDocumentosFiscaisResponse>> BuscarDocumentosFiscais(BuscarDocumentosFiscaisRequest request, string UsuarioId);
        Task<bool> ImportarDocumentoFiscalXml(string UsuarioId, string xml);
    }
}
