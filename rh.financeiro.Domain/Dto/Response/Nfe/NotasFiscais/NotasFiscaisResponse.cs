using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Response.Nfe.NotasFiscais
{
    using System.Text.Json.Serialization;

    public class NotasFiscaisResponse
    {
        [JsonPropertyName("notas_fiscais")]
        public List<NotaFiscal>? NotasFiscais { get; set; }

        [JsonPropertyName("pagination")]
        public Pagination? Pagination { get; set; }
    }

    public class NotaFiscal
    {
        public long Id { get; set; }

        [JsonPropertyName("xml")]
        public string? Xml { get; set; }

        [JsonPropertyName("xml_cancelamento")]
        public string? XmlCancelamento { get; set; }

        [JsonPropertyName("usuario")]
        public string? Usuario { get; set; }

        [JsonPropertyName("serie")]
        public int? Serie { get; set; }

        [JsonPropertyName("numero_nfe")]
        public int? NumeroNfe { get; set; }

        [JsonPropertyName("tipo_documento")]
        public int? TipoDocumento { get; set; }

        [JsonPropertyName("tipo_impressao_danfe")]
        public int? TipoImpressaoDanfe { get; set; }

        [JsonPropertyName("dthr_emissao")]
        public DateTime DthrEmissao { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("informacoes_complementares")]
        public string? InformacoesComplementares { get; set; }

        [JsonPropertyName("natureza_da_operacao")]
        public string? NaturezaDaOperacao { get; set; }

        [JsonPropertyName("protocolo")]
        public string? Protocolo { get; set; }

        [JsonPropertyName("motivo_cancelamento")]
        public string? MotivoCancelamento { get; set; }

        [JsonPropertyName("valor_frete")]
        public decimal? ValorFrete { get; set; }

        [JsonPropertyName("chave_acesso_nfe")]
        public string? ChaveAcessoNfe { get; set; }

        [JsonPropertyName("quantidade_volume")]
        public decimal? QuantidadeVolume { get; set; }

        [JsonPropertyName("especie_volumes")]
        public string? EspecieVolumes { get; set; }

        [JsonPropertyName("marca_volumes")]
        public string? MarcaVolumes { get; set; }

        [JsonPropertyName("numeracao_volumes")]
        public string? NumeracaoVolumes { get; set; }

        [JsonPropertyName("peso_liquido")]
        public decimal? PesoLiquido { get; set; }

        [JsonPropertyName("peso_bruto")]
        public decimal? PesoBruto { get; set; }

        [JsonPropertyName("valor_nota_nfe")]
        public decimal? ValorNotaNfe { get; set; }

        [JsonPropertyName("valor_total_produtos")]
        public decimal? ValorTotalProdutos { get; set; }

        [JsonPropertyName("valor_icms")]
        public decimal? ValorIcms { get; set; }

        [JsonPropertyName("valor_icms_st")]
        public decimal? ValorIcmsSt { get; set; }

        [JsonPropertyName("valor_ipi")]
        public decimal? ValorIpi { get; set; }

        [JsonPropertyName("valor_pis")]
        public decimal? ValorPis { get; set; }

        [JsonPropertyName("valor_cofins")]
        public decimal? ValorCofins { get; set; }

        [JsonPropertyName("valor_icms_bc")]
        public decimal? ValorIcmsBc { get; set; }

        [JsonPropertyName("valor_icms_st_bc")]
        public decimal? ValorIcmsStBc { get; set; }

        [JsonPropertyName("valor_ipi_bc")]
        public decimal? ValorIpiBc { get; set; }

        [JsonPropertyName("valor_pis_bc")]
        public decimal? ValorPisBc { get; set; }

        [JsonPropertyName("valor_cofins_bc")]
        public decimal? ValorCofinsBc { get; set; }

        [JsonPropertyName("valor_seguro")]
        public decimal? ValorSeguro { get; set; }

        [JsonPropertyName("valor_ii")]
        public decimal? ValorIi { get; set; }

        [JsonPropertyName("valor_ii_bc")]
        public decimal? ValorIiBc { get; set; }

        [JsonPropertyName("valor_desconto")]
        public decimal? ValorDesconto { get; set; }

        [JsonPropertyName("valor_fcp_uf_dest_final")]
        public decimal? ValorFcpUfDestFinal { get; set; }

        [JsonPropertyName("valor_icms_uf_dest_final")]
        public decimal? ValorIcmsUfDestFinal { get; set; }

        [JsonPropertyName("valor_icms_uf_remet_final")]
        public decimal? ValorIcmsUfRemetFinal { get; set; }

        [JsonPropertyName("valor_fcp")]
        public decimal? ValorFcp { get; set; }

        [JsonPropertyName("valor_fcp_st")]
        public decimal? ValorFcpSt { get; set; }

        [JsonPropertyName("valor_cofins_aproveitado")]
        public string? ValorCofinsAproveitado { get; set; }

        [JsonPropertyName("valor_pis_aproveitado")]
        public string? ValorPisAproveitado { get; set; }

        [JsonPropertyName("uf")]
        public Uf? Uf { get; set; }

        [JsonPropertyName("municipio")]
        public Municipio? Municipio { get; set; }

        [JsonPropertyName("cfop")]
        public Cfop? Cfop { get; set; }

        [JsonPropertyName("financeiro")]
        public Financeiro? Financeiro { get; set; }
    }

    public class Uf
    {
        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        [JsonPropertyName("nome_por_extenso")]
        public string? NomePorExtenso { get; set; }

        [JsonPropertyName("codigo_ibge")]
        public int? CodigoIbge { get; set; }
    }

    public class Municipio
    {
        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        [JsonPropertyName("codigo_ibge")]
        public int? CodigoIbge { get; set; }
    }

    public class Cfop
    {
        [JsonPropertyName("codigo")]
        public string? Codigo { get; set; }

        [JsonPropertyName("descricao")]
        public string? Descricao { get; set; }
    }

    public class Financeiro
    {
        [JsonPropertyName("valor_nota_nfe")]
        public decimal? ValorNotaNfe { get; set; }

        [JsonPropertyName("total_devolucoes")]
        public decimal? TotalDevolucoes { get; set; }

        [JsonPropertyName("valor_pedido_liquido")]
        public decimal? ValorPedidoLiquido { get; set; }

        [JsonPropertyName("total_faturado")]
        public decimal? TotalFaturado { get; set; }

        [JsonPropertyName("total_liquidado")]
        public decimal? TotalLiquidado { get; set; }

        [JsonPropertyName("falta_faturar")]
        public decimal? FaltaFaturar { get; set; }

        [JsonPropertyName("falta_liquidar")]
        public decimal? FaltaLiquidar { get; set; }

        [JsonPropertyName("recebimentos")]
        public List<object>? Recebimentos { get; set; }
    }

    public class Pagination
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("previous_page")]
        public string? PreviousPage { get; set; }

        [JsonPropertyName("next_page")]
        public string? NextPage { get; set; }
    }
}
