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
    [Table("movimento_bancario")]
    public class MovimentoBancario
    {
        [Key]
        [Column("id")]

        public Guid Id { get; set; }

        [Column("conta_id")]
        public Guid ContaId { get; set; }

        [Column("data")]
        public DateTime? Data { get; set; }

        [Column("tipo")]
        public TipoMovimento Tipo { get; set; }

        [Column("status")]
        public StatusMovimento Status { get; set; }

        [Column("valor_bruto")]
        public decimal? ValorBruto { get; set; }

        [Column("valor_liquido")]
        public decimal? ValorLiquido { get; set; }

        [Column("tarifa")]
        public decimal? Tarifa { get; set; }

        [Column("descricao")]
        public string Descricao { get; set; }

        [Column("conciliado")]
        public bool Conciliado { get; set; }

        [Column("identificador_externo")]
        public string? IdentificadorExterno { get; set; }

        [Column("hash_unico")]
        public string hashUnico { get; set; }

        [Column("created_at")]
        public DateTime createdAt { get; set; } = DateTime.Now.ToUniversalTime();
    }
}
