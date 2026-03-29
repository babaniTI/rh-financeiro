using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Enums
{
    public enum TipoParticipante
    {
        Cliente,
        Fornecedor,
        Transportadora
    }

    public enum TipoConta
    {
        Corrente,
        Poupanca,
        Caixa,
        Investimento,
        Cartao
    }

    public enum TipoDocumentoFiscal
    {
        NFE,
        NFSE,
        NFCE,
        CTE,
        OUTRO
    }

    public enum TipoCategoriaFinanceira
    {
        Receita,
        Despesa,
        Transferencia,
        Outro
    }

    public enum TipoTitulo
    {
        RECEBER,
        PAGAR
    }

    public enum StatusTitulo
    {
        Aberto,
        Parcial,
        Quitado,
        Cancelado
    }

    public enum StatusParcela
    {
        Aberto,
        Parcial,
        Pago,
        Cancelado
    }

    public enum TipoMovimento
    {
        Credito,
        Debito
    }

    public enum TipoConciliacao
    {
        Total,
        Parcial
    }

    public enum TipoEventoFinanceiro
    {
        Pagamento,
        Estorno,
        Chargeback,
        Cancelamento,
        Ajuste
    }

    public enum TipoLiquidacao
    {
        Pagamento,
        Split,
        Retencao,
        Compensacao,
        Ajuste
    }

    public enum TipoTributo
    {
        IBS,
        CBS,
        IS
    }
}
