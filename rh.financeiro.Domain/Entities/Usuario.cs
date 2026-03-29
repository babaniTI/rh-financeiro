using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Entities
{
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("senha_hash")]
        public string SenhaHash { get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
