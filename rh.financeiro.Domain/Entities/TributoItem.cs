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
    [Table("tributo_item")]
    public class TributoItem
    {
        [Key]
        public Guid Id { get; set; }

        [Column("item_id")]
        public Guid ItemId { get; set; }

        [Column("tipo")]
        public TipoTributo Tipo { get; set; }

        [Column("base")]
        public decimal? Base { get; set; }

        [Column("aliquota")]
        public decimal? Aliquota { get; set; }

        [Column("valor")]
        public decimal? Valor { get; set; }
    }
}
