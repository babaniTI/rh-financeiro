using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Entities
{
    [Table("auditoria")]
    public class Auditoria
    {
        [Key]
        public Guid Id { get; set; }

        [Column("entidade")]
        public string Entidade { get; set; }

        [Column("entidade_id")]
        public Guid? EntidadeId { get; set; }

        [Column("acao")]
        public string Acao { get; set; }

        [Column("dados_antes")]
        public string DadosAntes { get; set; }

        [Column("dados_depois")]
        public string DadosDepois { get; set; }

        [Column("usuario")]
        public string Usuario { get; set; }

        [Column("origem")]
        public string Origem { get; set; }

        [Column("versao")]
        public int? Versao { get; set; }
    }
}
