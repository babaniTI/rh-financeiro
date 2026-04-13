using rh.financeiro.Domain.Dto.Paginacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Request.MovimentacaoBancaria.BuscarMovimentacoesBancarias
{
    public class BuscarMovimentacoesBancariasRequest : PaginacaoRequest
    {
        public string? contaFinanceiraId { get; set; }
        public string? tipo { get; set; }
        public string? status { get; set; }
        public string? dataInicio { get; set; }
        public string? dataFim { get; set;}
        public string? search { get; set; }
    }
}
