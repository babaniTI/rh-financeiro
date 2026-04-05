using rh.financeiro.Domain.Dto.Response.Nfe.NotasFiscais;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Interfaces.Service.Nfe
{
    public interface INfeService
    {
        Task<List<NotaFiscal>> BuscarDocumentosEmitidos(DateTime inicio, DateTime fim, string Cnpj);
        Task<List<NotaFiscal>> BuscarDocumentoPorNumeroNfes(List<string> NumerosNfe, string Cnpj);
    }
}
