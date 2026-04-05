using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Request.CategoriaFinanceira.EditarCategoriaFinanceira
{
    public class EditarCategoriaFinanceiraRequest
    {
        public string codigo { get; set; }
        public string cfop { get; set; }
        public string descricao { get; set; }
        public string tipo { get; set; }
    }
}
