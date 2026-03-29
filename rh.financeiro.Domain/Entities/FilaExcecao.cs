using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Entities
{
    [Table("fila_excecao")]
    public class FilaExcecao
    {
        [Key]
        public Guid Id { get; set; }

        [Column("tipo")]
        public string Tipo { get; set; }

        [Column("entidade_id")]
        public Guid? EntidadeId { get; set; }

        [Column("descricao")]
        public string Descricao { get; set; }

        [Column("status")]
        public string Status { get; set; }
    }
}
