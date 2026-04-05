using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Enums
{
    public enum MeioPagamentoSefaz
    {
        Dinheiro = 1,                // 01
        Cheque = 2,                  // 02
        CartaoCredito = 3,           // 03
        CartaoDebito = 4,            // 04
        CreditoLoja = 5,             // 05
        ValeAlimentacao = 10,        // 10
        ValeRefeicao = 11,           // 11
        ValePresente = 12,           // 12
        ValeCombustivel = 13,        // 13
        BoletoBancario = 15,         // 15
        DepositoBancario = 16,       // 16
        Pix = 17,                    // 17
        TransferenciaBancaria = 18,  // 18 (inclui TED/DOC)
        ProgramaFidelidade = 19,     // 19 (cashback, pontos, etc)
        SemPagamento = 90,           // 90
        Outros = 99                  // 99
    }
}
