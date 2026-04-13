using rh.financeiro.Domain.Entities;
using rh.financeiro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Response.MovimentacaoBancaria.BuscarMovimentacoesBancarias
{
    public class BuscarMovimentacoesBancariasResponse
    {
        public string? Id { get; set; }
        public DateTime? Data { get; set; }
        public string? Conta { get; set; }
        public string? Descricao { get; set; }
        public decimal? ValorBruto { get; set; }
        public decimal? Tarifa { get; set; }
        public decimal? ValorLiquido { get; set; }
        public StatusMovimento? Status { get; set; }
    }
}
