using rh.financeiro.Domain.Dto.Paginacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Request.DocumentoFiscal.BuscarDocumentosFiscais
{
    public class BuscarDocumentosFiscaisRequest : PaginacaoRequest
    {
        public string? tipo { get; set; }
        public string? status { get; set; } 
        public string? dataEmissaoInicio { get; set; }
        public string? dataEmissaoFim { get; set;}
        public string? search { get; set; }
    }
}
