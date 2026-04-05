using rh.financeiro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Request.ContasFinanceiras.CriarContaFinanceira
{
    public class CriarContaFinanceiraRequest
    {
        public string? descricao { get; set; }
        public string tipo { get; set; } 
        public string? banco { get; set; }
        public string? agencia { get; set; }
        public string? agenciaDigito { get; set; }
        public string? conta { get; set; }  
        public string? contaDigito { get; set; }
        public decimal? saldoInicial { get; set; }
    }
}
