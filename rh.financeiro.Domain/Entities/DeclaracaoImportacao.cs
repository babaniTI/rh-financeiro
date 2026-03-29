using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Entities
{
    [Table("declaracao_importacao")]
    public class DeclaracaoImportacao
    {
        [Key]
        public Guid Id { get; set; }

        [Column("empresa_id")]
        public Guid EmpresaId { get; set; }

        [Column("numero")]
        public string Numero { get; set; }

        [Column("data")]
        public DateTime? Data { get; set; }
    }
}
