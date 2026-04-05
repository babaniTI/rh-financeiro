using rh.financeiro.Domain.Entities;
using rh.financeiro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Response.DocumentoFiscal.BuscarDocumentosFiscais
{
    public class BuscarDocumentosFiscaisResponse
    {
        public string? Id { get; set; }
        public TipoDocumentoFiscal? Tipo { get; set; }
        public string? Numero { get; set; }
        public string? Serie { get; set; }
        public string? ChaveAcesso { get; set; }
        public DateTime? DataEmissao { get; set; }
        public DateTime? DataEntrada { get; set; }
        public decimal? ValorProdutos { get; set; }
        public decimal? ValorServicos { get; set; }
        public decimal? ValorFrete { get; set; }
        public decimal? ValorSeguro { get; set; }
        public decimal? ValorDesconto { get; set; }
        public decimal? ValorOutros { get; set; }
        public decimal? ValorTotal { get; set; }
        public TipoStatusDocumentoFiscal? Status { get; set; }
        public string? TituloId { get; set; }
        public List<ItemDocumentoFiscal>? Itens { get; set; } = new();
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ItemDocumentoFiscal
    {
        public string? Id { get; set; }
        public string? DocumentoId { get; set; }
        public int? Sequencia { get; set; }
        public string? CodigoProduto { get; set; }
        public string? Descricao { get; set; }
        public string? Ncm { get; set; }
        public string? Cfop { get; set; }
        public string? Unidade { get; set; }
        public decimal? Quantidade { get; set; }
        public decimal? ValorUnitario { get; set; }
        public decimal? ValorTotal { get; set; }
        public decimal? ValorDesconto { get; set; }
        public ImpostosItem? Impostos { get; set; }
    }

    public class ImpostosItem
    {
        // Reforma Tributária
        public ImpostoDetalhe? Ibs { get; set; }
        public ImpostoDetalhe? Cbs { get; set; }
        public ImpostoDetalhe? Is { get; set; }
    }

    public class ImpostoDetalhe
    {
        public decimal? Base { get; set; }
        public decimal? Aliquota { get; set; }
        public decimal? Valor { get; set; }
    }
}
