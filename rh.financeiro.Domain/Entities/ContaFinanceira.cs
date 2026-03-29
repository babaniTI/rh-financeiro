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
    [Table("conta_financeira")]
    public class ContaFinanceira
    {
        [Key]
        public Guid Id { get; set; }

        [Column("empresa_id")]
        public Guid EmpresaId { get; set; }

        [Column("tipo")]
        public TipoConta Tipo { get; set; }

        [Column("banco")]
        public string Banco { get; set; }

        [Column("agencia")]
        public string Agencia { get; set; }

        [Column("conta")]
        public string Conta { get; set; }

        [Column("descricao")]
        public string Descricao { get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
