using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Entities
{
    [Table("rateio")]
    public class Rateio
    {
        [Key]
        public Guid Id { get; set; }

        [Column("lancamento_id")]
        public Guid LancamentoId { get; set; }

        [Column("centro_custo")]
        public string CentroCusto { get; set; }

        [Column("valor")]
        public decimal? Valor { get; set; }
    }
}
