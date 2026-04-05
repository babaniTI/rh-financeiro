using rh.financeiro.Domain.Entities;
using rh.financeiro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Response.Titulos.BuscarTitulos
{
    public class BuscarTitulosResponse
    {
        public Guid? Id { get; set; }
        public Guid? EmpresaId { get; set; }
        public TipoTitulo? Tipo { get; set; }
        public string? Numero { get; set; } = string.Empty;
        public string? NomeParticipante { get; set; }
        public DateTime? DataEmissao { get; set; }
        public decimal? ValorTotal { get; set; }
        public decimal? ValorPago { get; set; }
        public decimal? ValorAtual { get; set; }
        public StatusTitulo? Status { get; set; }
        public List<Parcela>? Parcelas { get; set; } = new();
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
