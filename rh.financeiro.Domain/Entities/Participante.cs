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
    [Table("participante")]
    public class Participante
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("empresa_id")]
        public Guid EmpresaId { get; set; }

        [Column("tipo")]
        public TipoParticipante Tipo { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("documento")]
        public string Documento { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
