using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Request.CategoriaFinanceira.CriarCategoriaFinanceira
{
    public class CriarCategoriaFinanceiraRequest
    {
        public string codigo { get; set; }
        public string Cfop { get; set; }
        public string? descricao { get; set; }
        public string tipo { get; set; }
    }
}
