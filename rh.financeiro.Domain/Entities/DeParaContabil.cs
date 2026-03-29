using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Entities
{
    [Table("de_para_contabil")]
    public class DeParaContabil
    {
        [Key]
        public Guid Id { get; set; }

        [Column("empresa_id")]
        public Guid EmpresaId { get; set; }

        [Column("categoria_financeira_id")]
        public Guid? CategoriaId { get; set; }

        [Column("evento_financeiro")]
        public string EventoFinanceiro { get; set; }

        [Column("conta_debito_provisao")]
        public string ContaDebitoProvisao { get; set; }

        [Column("conta_credito_provisao")]
        public string ContaCreditoProvisao { get; set; }

        [Column("conta_debito_baixa")]
        public string ContaDebitoBaixa { get; set; }

        [Column("conta_credito_baixa")]
        public string ContaCreditoBaixa { get; set; }

        [Column("historico_codigo")]
        public string HistoricoCodigo { get; set; }

        [Column("historico_padrao")]
        public string HistoricoPadrao { get; set; }

        [Column("centro_custo_obrigatorio")]
        public bool? CentroCustoObrigatorio { get; set; }

        [Column("participante_obrigatorio")]
        public bool? ParticipanteObrigatorio { get; set; }

        [Column("permite_rateio")]
        public bool? PermiteRateio { get; set; }

        [Column("natureza_fiscal")]
        public string NaturezaFiscal { get; set; }

        [Column("versao")]
        public int? Versao { get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; }
    }
}
