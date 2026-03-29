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
    [Table("liquidacao_financeira")]
    public class LiquidacaoFinanceira
    {
        [Key]
        public Guid Id { get; set; }

        [Column("parcela_id")]
        public Guid ParcelaId { get; set; }

        [Column("tipo")]
        public TipoLiquidacao Tipo { get; set; }

        [Column("valor")]
        public decimal? Valor { get; set; }

        [Column("data")]
        public DateTime? Data { get; set; }
    }
}
