using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Entities
{
    [Table("empresa")]
    public class Empresa
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("cnpj")]
        public string Cnpj { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
