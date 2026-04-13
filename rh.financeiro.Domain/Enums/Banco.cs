using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace rh.financeiro.Domain.Enums
{
    public enum TipoParticipante
    {
        [Description("cliente")] Cliente,
        [Description("fornecedor")] Fornecedor,
        [Description("transportadora")] Transportadora
    }

    public enum TipoConta
    {
        [Description("corrente")] Corrente,
        [Description("poupanca")] Poupanca,
        [Description("caixa")] Caixa,
        [Description("investimento")] Investimento,
        [Description("cartao")] Cartao
    }

    public enum TipoDocumentoFiscal
    {
        [Description("NFE")] NFE,
        [Description("NFSE")] NFSE,
        [Description("NFCE")] NFCE,
        [Description("CTE")] CTE,
        [Description("OUTRO")] OUTRO
    }

    public enum TipoStatusDocumentoFiscal
    {
        [Description("pendente")] Pendente,
        [Description("processado")] Processado,
        [Description("cancelado")] Cancelado
    }

    public enum TipoCategoriaFinanceira
    {
        [Description("receita")] Receita,
        [Description("despesa")] Despesa,
        [Description("transferencia")] Transferencia,
        [Description("outro")] Outro
    }

    public enum TipoTitulo
    {
        [Description("RECEBER")] RECEBER,
        [Description("PAGAR")] PAGAR
    }

    public enum StatusTitulo
    {
        [Description("aberto")] Aberto,
        [Description("parcial")] Parcial,
        [Description("quitado")] Quitado,
        [Description("cancelado")] Cancelado
    }

    public enum StatusParcela
    {
        [Description("aberto")] Aberto,
        [Description("parcial")] Parcial,
        [Description("pago")] Pago,
        [Description("cancelado")] Cancelado
    }

    public enum TipoMovimento
    {
        [Description("credito")] Credito,
        [Description("debito")] Debito
    }
    public enum StatusMovimento
    {
        [Description("PENDENTE")] PENDENTE,
        [Description("CONCILIADO")] CONCILIADO,
        [Description("PARCIAL")] PARCIAL
    }

    public enum TipoConciliacao
    {
        [Description("total")] Total,
        [Description("parcial")] Parcial
    }

    public enum TipoEventoFinanceiro
    {
        [Description("pagamento")] Pagamento,
        [Description("estorno")] Estorno,
        [Description("chargeback")] Chargeback,
        [Description("cancelamento")] Cancelamento,
        [Description("ajuste")] Ajuste
    }

    public enum TipoLiquidacao
    {
        [Description("pagamento")] Pagamento,
        [Description("split")] Split,
        [Description("retencao")] Retencao,
        [Description("compensacao")] Compensacao,
        [Description("ajuste")] Ajuste
    }

    public enum TipoTributo
    {
        [Description("IBS")] IBS,
        [Description("CBS")] CBS,
        [Description("IS")] IS
    }

    public static class EnumExtensions
    {
        public static string ToDbString<T>(this T value) where T : Enum
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? value.ToString().ToLowerInvariant();
        }

        // String -> Enum (ex: "receita" => TipoCategoriaFinanceira.Receita)
        public static T ToEnum<T>(this string value) where T : Enum
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));

            var type = typeof(T);

            // 1. Tenta encontrar pelo atributo [Description] (case-insensitive)
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attr = field.GetCustomAttribute<DescriptionAttribute>();
                if (attr != null && attr.Description.Equals(value, StringComparison.OrdinalIgnoreCase))
                    return (T)field.GetValue(null);
            }

            // 2. Fallback compatível: tenta converter pelo nome do enum (case-insensitive)
            // Usamos Enum.GetNames + IndexOf para evitar problemas de TryParse genérico em .NET Framework
            foreach (var name in Enum.GetNames(type))
            {
                if (name.Equals(value, StringComparison.OrdinalIgnoreCase))
                    return (T)Enum.Parse(type, name);
            }

            throw new ArgumentException($"'{value}' não é um valor válido para {type.Name}.");
        }
    }
}