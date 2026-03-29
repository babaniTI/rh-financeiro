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
    [Table("categoria_financeira")]
    public class CategoriaFinanceira
    {
        [Key]
        public Guid Id { get; set; }

        [Column("empresa_id")]
        public Guid EmpresaId { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("tipo")]
        public TipoCategoriaFinanceira Tipo { get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; }
    }
}
