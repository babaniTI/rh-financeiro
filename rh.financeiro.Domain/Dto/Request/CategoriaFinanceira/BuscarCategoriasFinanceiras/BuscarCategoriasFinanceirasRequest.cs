using rh.financeiro.Domain.Dto.Paginacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Request.CategoriaFinanceira.BuscarCategoriasFinanceiras
{
    public class BuscarCategoriasFinanceirasRequest : PaginacaoRequest
    {
        public string? tipo { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
