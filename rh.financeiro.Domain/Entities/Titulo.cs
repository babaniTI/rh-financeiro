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
    [Table("titulo")]
    public class Titulo
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("empresa_id")]
        public Guid EmpresaId { get; set; }

        [Column("tipo")]
        public TipoTitulo Tipo { get; set; }

        [Column("documento_id")]
        public Guid? DocumentoId { get; set; }

        [Column("participante_id")]
        public Guid? ParticipanteId { get; set; }

        [Column("categoria_financeira_id")]
        public Guid? CategoriaId { get; set; }

        [Column("valor_total")]
        public decimal ValorTotal { get; set; }

        [Column("data_emissao")]
        public DateTime? DataEmissao { get; set; }

        [Column("status")]
        public StatusTitulo Status { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}
