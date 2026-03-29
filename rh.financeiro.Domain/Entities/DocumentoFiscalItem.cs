using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Entities
{
    [Table("documento_fiscal_item")]
    public class DocumentoFiscalItem
    {
        [Key]
        public Guid Id { get; set; }

        [Column("documento_id")]
        public Guid DocumentoId { get; set; }

        [Column("produto_codigo")]
        public string ProdutoCodigo { get; set; }

        [Column("descricao")]
        public string Descricao { get; set; }

        [Column("quantidade")]
        public decimal? Quantidade { get; set; }

        [Column("valor_unitario")]
        public decimal? ValorUnitario { get; set; }

        [Column("valor_total")]
        public decimal? ValorTotal { get; set; }
    }
}
