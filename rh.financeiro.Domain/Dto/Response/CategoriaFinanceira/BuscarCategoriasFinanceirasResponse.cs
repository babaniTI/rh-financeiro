using rh.financeiro.Domain.Dto.Paginacao;
using rh.financeiro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Response.CategoriaFinanceira
{
    public class BuscarCategoriasFinanceirasResponse 
    {
        public string id { get; set; }  
        public string codigo { get; set; }
        public string cfop { get; set; }

        public string descricao { get; set; }
        public string tipo { get; set; }
        public bool ativo { get; set; }
    }
}
