using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Entities
{
    [Table("usuario_empresa")]
    public class UsuarioEmpresa
    {
        [Key]
        public Guid Id { get; set; }

        [Column("usuario_id")]
        public Guid UsuarioId { get; set; }

        [Column("empresa_id")]
        public Guid EmpresaId { get; set; }

        [Column("role")]
        public string Role { get; set; }
    }
}
