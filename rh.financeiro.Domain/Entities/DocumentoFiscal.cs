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
    [Table("documento_fiscal")]
    public class DocumentoFiscal
    {
        [Key]
        public Guid Id { get; set; }

        [Column("empresa_id")]
        public Guid EmpresaId { get; set; }

        [Column("tipo")]
        public TipoDocumentoFiscal Tipo { get; set; }

        [Column("numero")]
        public string Numero { get; set; }

        [Column("serie")]
        public string Serie { get; set; }

        [Column("chave")]
        public string Chave { get; set; }

        [Column("participante_id")]
        public Guid? ParticipanteId { get; set; }

        [Column("data_emissao")]
        public DateTime? DataEmissao { get; set; }

        [Column("valor_total")]
        public decimal? ValorTotal { get; set; }

        [Column("dados_tributarios")]
        public string DadosTributarios { get; set; }
    }
}
