using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Entities
{
    [Table("lancamento_contabil")]
    public class LancamentoContabil
    {
        [Key]
        public Guid Id { get; set; }

        [Column("empresa_id")]
        public Guid EmpresaId { get; set; }

        [Column("data")]
        public DateTime Data { get; set; }

        [Column("conta_debito")]
        public string ContaDebito { get; set; }

        [Column("conta_credito")]
        public string ContaCredito { get; set; }

        [Column("valor")]
        public decimal? Valor { get; set; }

        [Column("historico_codigo")]
        public string HistoricoCodigo { get; set; }

        [Column("historico_complemento")]
        public string HistoricoComplemento { get; set; }

        [Column("documento_id")]
        public Guid? DocumentoId { get; set; }

        [Column("titulo_id")]
        public Guid? TituloId { get; set; }

        [Column("parcela_id")]
        public Guid? ParcelaId { get; set; }

        [Column("movimento_id")]
        public Guid? MovimentoId { get; set; }

        [Column("lote")]
        public string Lote { get; set; }

        [Column("exportado")]
        public bool Exportado { get; set; }
    }
}
