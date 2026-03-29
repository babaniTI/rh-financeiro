using rh.financeiro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Entities
{
    [Table("evento_financeiro")]
    public class EventoFinanceiro
    {
        [Key]
        public Guid Id { get; set; }

        [Column("parcela_id")]
        public Guid? ParcelaId { get; set; }

        [Column("movimento_id")]
        public Guid? MovimentoId { get; set; }

        [Column("tipo")]
        public TipoEventoFinanceiro Tipo { get; set; }

        [Column("valor")]
        public decimal? Valor { get; set; }

        [Column("descricao")]
        public string Descricao { get; set; }
    }
}
