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
    [Table("parcela")]
    public class Parcela
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("titulo_id")]
        public Guid TituloId { get; set; }

        [Column("numero")]
        public int Numero { get; set; }

        [Column("data_vencimento")]
        public DateTime? DataVencimento { get; set; }

        [Column("valor")]
        public decimal? Valor { get; set; }

        [Column("saldo")]
        public decimal? Saldo { get; set; }

        [Column("status")]
        public StatusParcela Status { get; set; }

        [Column("descricao_pagamento")]
        public string? DescricaoPagamento { get; set; }
    }
}
