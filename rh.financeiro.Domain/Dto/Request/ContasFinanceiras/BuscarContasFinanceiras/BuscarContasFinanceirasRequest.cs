using rh.financeiro.Domain.Dto.Paginacao;
using rh.financeiro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Request.ContasFinanceiras.BuscarContasFinanceiras
{
    public class BuscarContasFinanceirasRequest : PaginacaoRequest
    {
        public TipoConta? tipo { get; set; }
    }
}
