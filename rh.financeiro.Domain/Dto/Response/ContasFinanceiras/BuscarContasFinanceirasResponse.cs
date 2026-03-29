using rh.financeiro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Response.ContasFinanceiras
{
    public class BuscarContasFinanceirasResponse
    {
        public string id { get; set; }
        public string? descricao { get; set; }
        public TipoConta tipo { get; set; }
        public string? banco { get; set; }
        public string? agencia { get; set; }
        public string? agenciaDigito { get; set; }
        public string? conta { get; set; }
        public string? contaDigito { get; set; }
        public decimal? saldoAtual { get; set; }
        public string dataUltimoMovimento { get; set; } // ISO 8601
        public bool Ativo { get; set; }
        public string createdAt { get; set; } // ISO 8601
        public string updatedAt { get; set; } // ISO 8601
    }
}
