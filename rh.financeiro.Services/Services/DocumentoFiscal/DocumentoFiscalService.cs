using Microsoft.EntityFrameworkCore;
using rh.financeiro.CrossCuting;
using rh.financeiro.Data.Context.Default;
using rh.financeiro.Domain.Common.DocumentosNfes;
using rh.financeiro.Domain.Dto.Paginacao;
using rh.financeiro.Domain.Dto.Request.DocumentoFiscal.BuscarDocumentosFiscais;
using rh.financeiro.Domain.Dto.Response.CategoriaFinanceira;
using rh.financeiro.Domain.Dto.Response.ContasFinanceiras;
using rh.financeiro.Domain.Dto.Response.DocumentoFiscal.BuscarDocumentosFiscais;
using rh.financeiro.Domain.Entities;
using rh.financeiro.Domain.Enums;
using rh.financeiro.Domain.Interfaces.Service.DocumentoFiscal;
using rh.financeiro.Domain.Interfaces.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace rh.financeiro.Services.Services.DocumentoFiscais
{
    public class DocumentoFiscalService : IDocumentoFiscalService
    {
        #region Fields
        private readonly IUnitOfWork<DefaultContext> _unitOfWork;
        #endregion

        #region CONS
        public DocumentoFiscalService(IUnitOfWork<DefaultContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Public Methods
        public async Task<PaginacaoResponse<BuscarDocumentosFiscaisResponse>> BuscarDocumentosFiscais(BuscarDocumentosFiscaisRequest request, string UsuarioId)
        {
            #region Fields
            var EmpresaId = await BuscarEmpresaIdPorUsarioId(UsuarioId);
            var repoDocumentoFiscal = _unitOfWork.Repository<DocumentoFiscal>().QueryableObject();
            var repoDocumentoXItem = _unitOfWork.Repository<DocumentoFiscalItem>().QueryableObject();
            var repoTributoItem = _unitOfWork.Repository<TributoItem>().QueryableObject();
            #endregion

            #region Query
            var query =
                from df in repoDocumentoFiscal
                join dxi in repoDocumentoXItem
                    on df.Id equals dxi.DocumentoId into itens
                where df.EmpresaId == EmpresaId
                select new
                {
                    Documento = df,
                    Itens = from item in itens
                            join ti in repoTributoItem
                                on item.Id equals ti.ItemId into tributos
                            select new
                            {
                                Item = item,
                                Tributos = tributos
                            }
                };
            #endregion

            #region Logic
            try
            {
                if (!string.IsNullOrEmpty(request.tipo))
                    query = query.Where(x => x.Documento.Tipo.ToString() == request.tipo);

                if (!string.IsNullOrEmpty(request.status))
                    query = query.Where(x => x.Documento.status.ToString() == request.status);

                #region Filtro por Data de Emissão (ISO 8601)
                DateTimeOffset? dataInicio = string.IsNullOrEmpty(request.dataEmissaoInicio)
                    ? null
                    : DateTimeOffset.Parse(request.dataEmissaoInicio);

                DateTimeOffset? dataFim = string.IsNullOrEmpty(request.dataEmissaoFim)
                     ? null
                     : DateTimeOffset.Parse(request.dataEmissaoFim)
                        .Date
                        .AddDays(1)
                        .AddTicks(-1);

                if (dataInicio.HasValue || dataFim.HasValue)
                {
                    query = query.Where(x =>
                        (!dataInicio.HasValue || x.Documento.DataEmissao >= dataInicio.Value.DateTime) &&
                        (!dataFim.HasValue || x.Documento.DataEmissao <= dataFim.Value.DateTime)
                    );
                }
                #endregion

                if (!string.IsNullOrEmpty(request.search))
                {
                    var search = request.search.Trim();
                    query = query.Where(x =>
                        (search.Length == 44 && x.Documento.Chave == search)
                        ||
                        x.Documento.Numero.ToString().Equals(search)
                    );
                }

                var total = await query.CountAsync();

                // Paginação
                var page = request.page <= 0 ? 1 : request.page;
                var pageSize = request.pageSize <= 0 ? 10 : request.pageSize;

                var data = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new
                    {
                        Documento = x.Documento,
                        Itens = x.Itens.Select(i => new
                        {
                            Item = i.Item,
                            Tributos = i.Tributos
                        })
                    })
                    .ToListAsync();

                var result = data.Select(x => new BuscarDocumentosFiscaisResponse
                {
                    Id = x.Documento.Id.ToString(),
                    Tipo = x.Documento.Tipo,
                    Numero = x.Documento.Numero,
                    Serie = x.Documento.Serie,
                    ChaveAcesso = x.Documento.Chave,
                    DataEmissao = x.Documento.DataEmissao,
                    DataEntrada = x.Documento.DataEmissao,
                    Status = x.Documento.status,
                    CreatedAt = x.Documento.Created_at,
                    UpdatedAt = x.Documento.Updated_at,
                    ValorProdutos = x.Itens.Sum(i => i.Item.ValorTotal),
                    ValorTotal = x.Itens.Sum(i => i.Item.ValorTotal),
                    Itens = x.Itens
                        .Select((item, index) => new ItemDocumentoFiscal
                        {
                            Id = item.Item.Id.ToString(),
                            DocumentoId = x.Documento.Id.ToString(),
                            Sequencia = index,
                            CodigoProduto = item.Item.ProdutoCodigo,
                            Descricao = item.Item.Descricao,
                            Ncm = item.Item.Ncm,
                            Cfop = item.Item.Cfop,
                            Quantidade = item.Item.Quantidade,
                            ValorUnitario = item.Item.ValorUnitario,
                            ValorTotal = item.Item.ValorTotal,
                            Impostos = new()
                            {
                                Ibs = new()
                                {
                                    Base = item.Tributos.Where(x => x.Tipo == TipoTributo.IBS).Select(x => x.Base).FirstOrDefault(),
                                    Aliquota = item.Tributos.Where(x => x.Tipo == TipoTributo.IBS).Select(x => x.Aliquota).FirstOrDefault(),
                                    Valor = item.Tributos.Where(x => x.Tipo == TipoTributo.IBS).Select(x => x.Valor).FirstOrDefault(),
                                },

                                Cbs = new()
                                {
                                    Base = item.Tributos.Where(x => x.Tipo == TipoTributo.CBS).Select(x => x.Base).FirstOrDefault(),
                                    Aliquota = item.Tributos.Where(x => x.Tipo == TipoTributo.CBS).Select(x => x.Aliquota).FirstOrDefault(),
                                    Valor = item.Tributos.Where(x => x.Tipo == TipoTributo.CBS).Select(x => x.Valor).FirstOrDefault(),
                                }
                            }
                        })
                        .ToList()

                }).ToList();

                return new PaginacaoResponse<BuscarDocumentosFiscaisResponse>
                {
                    total = total,
                    page = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling((double)total / pageSize),
                    data = result
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            #endregion

        }

        public async Task<bool> ImportarDocumentoFiscalXml(string UsuarioId, string xml)
        {
            #region Fields
            Guid EmpresaId = await BuscarEmpresaIdPorUsarioId(UsuarioId);
            #endregion

            #region Logic
            try
            {
                // 1. Converter XML → JSON
                var json = Utils.ConverterXmlParaJson(xml);

                // 2. Desserializar
                var documento = JsonSerializer.Deserialize<NfeProc>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (documento?.NFe?.infNFe == null)
                    throw new Exception("XML inválido ou fora do padrão NFe/NFCe");

                // 3. Identificar chave de acesso
                string DocumentoChaveAcesso = documento?.NFe?.infNFe?.Id?.Replace("NFe", "");

                // 4. Identificar status da nota
                string NotaStatus = "ET"; // default

                if (documento?.protNFe?.infProt?.cStat == "100")
                    NotaStatus = "A"; // Autorizado

                if (documento?.protNFe?.infProt?.cStat == "101" ||
                    documento?.protNFe?.infProt?.cStat == "135" ||
                    documento?.protNFe?.infProt?.cStat == "155")
                    NotaStatus = "C"; // Cancelado

                // 5. Participante
                Participante participante = null;

                // Caso tenha informacoes do consumidor, cria um participante
                if (documento?.NFe?.infNFe?.dest != null)
                {
                    string CPfCNPJ = documento?.NFe?.infNFe?.dest?.CPF ?? documento?.NFe?.infNFe?.dest?.CNPJ;

                    // Verificar se ja existe na base
                    bool ExisteParticipante = await _unitOfWork.Repository<Participante>()
                        .QueryableObject()
                        .AnyAsync(x => x.Documento == CPfCNPJ);

                    // Se nao existe, cria!
                    if (!ExisteParticipante)
                    {
                        participante = new Participante()
                        {
                            EmpresaId = EmpresaId,
                            Tipo = TipoParticipante.Cliente,
                            Nome = documento?.NFe?.infNFe?.dest?.xNome,
                            Documento = CPfCNPJ,
                        };

                        await _unitOfWork.Repository<Participante>().AddAsync(participante);
                    }
                }

                // 6. Documento Fiscal
                var DocumentoFiscal = new DocumentoFiscal()
                {
                    EmpresaId = EmpresaId,
                    Tipo = documento?.NFe?.infNFe?.ide?.mod == "65"
                        ? TipoDocumentoFiscal.NFCE
                        : TipoDocumentoFiscal.NFE,

                    Numero = documento?.NFe?.infNFe?.ide?.nNF,
                    Serie = documento?.NFe?.infNFe?.ide?.serie,
                    Chave = DocumentoChaveAcesso,
                    ParticipanteId = participante?.Id,
                    DataEmissao = documento?.NFe?.infNFe?.ide?.dhEmi,

                    ValorTotal = documento?.NFe?.infNFe?.det?.Sum(x => x.prod?.vProd),

                    DadosTributarios = json,

                    status = NotaStatus switch
                    {
                        "A" => TipoStatusDocumentoFiscal.Processado,
                        "C" => TipoStatusDocumentoFiscal.Cancelado,
                        _ => TipoStatusDocumentoFiscal.Pendente
                    }
                };

                await _unitOfWork.Repository<DocumentoFiscal>().AddAsync(DocumentoFiscal);

                // 7. Itens
                var itens = documento?.NFe?.infNFe?.det ?? new List<Det>();
                var documentoFiscalItems = new List<DocumentoFiscalItem>();

                foreach (var item in itens)
                {
                    var DocumentoItem = new DocumentoFiscalItem()
                    {
                        DocumentoId = DocumentoFiscal.Id,
                        ProdutoCodigo = item?.prod?.cProd,
                        Descricao = item?.prod?.xProd,
                        Ncm = item?.prod?.NCM,
                        Cfop = item?.prod?.CFOP,
                        Quantidade = item?.prod?.qCom,
                        ValorUnitario = item?.prod?.vProd / item?.prod?.qCom,
                        ValorTotal = item?.prod?.vProd,
                    };

                    documentoFiscalItems.Add(DocumentoItem);
                }

                await _unitOfWork.Repository<DocumentoFiscalItem>().AddRangeAsync(documentoFiscalItems);

                // 8. Tributos
                var TributoItems = new List<TributoItem>();

                foreach (var item in itens)
                {
                    var documentoItem = documentoFiscalItems.FirstOrDefault(x =>
                        x.ProdutoCodigo == item?.prod?.cProd &&
                        x.ValorTotal == item?.prod?.vProd
                    );

                    if (documentoItem == null) continue;

                    var ibsCbs = item?.imposto?.IBSCBS?.gIBSCBS;

                    if (ibsCbs != null)
                    {
                        // CBS
                        TributoItems.Add(new TributoItem()
                        {
                            ItemId = documentoItem.Id,
                            Tipo = TipoTributo.CBS,
                            Base = ibsCbs?.vBC,
                            Aliquota = ibsCbs?.gCBS?.pCBS,
                            Valor = ibsCbs?.gCBS?.vCBS,
                        });

                        // IBS
                        TributoItems.Add(new TributoItem()
                        {
                            ItemId = documentoItem.Id,
                            Tipo = TipoTributo.IBS,
                            Base = ibsCbs?.vBC,
                            Aliquota =
                                (ibsCbs?.gIBSUF?.pIBSUF ?? 0) +
                                (ibsCbs?.GIBSMun?.pIBSMun ?? 0),

                            Valor =
                                (ibsCbs?.gIBSUF?.vIBSUF ?? 0) +
                                (ibsCbs?.GIBSMun?.vIBSMun ?? 0),
                        });
                    }
                }

                await _unitOfWork.Repository<TributoItem>().AddRangeAsync(TributoItems);

                // 9. Titulo
                string cfop = itens.First()?.prod?.CFOP;
                Guid CategoriaFinanceiraId = await BuscarCategoriaIdPorCfop(cfop);
                var Titulo = new Titulo()
                {
                    EmpresaId = EmpresaId,
                    Tipo = documento?.NFe?.infNFe?.ide?.mod == "65"
                        ? TipoTitulo.RECEBER
                        : TipoTitulo.PAGAR,
                    DocumentoId = DocumentoFiscal.Id,
                    ParticipanteId = DocumentoFiscal.ParticipanteId,
                    CategoriaId = CategoriaFinanceiraId,
                    ValorTotal = (decimal)DocumentoFiscal.ValorTotal!,
                    DataEmissao = DocumentoFiscal.DataEmissao,
                    Status = ClassificarStatusTitulo(documento, NotaStatus)
                };

                await _unitOfWork.Repository<Titulo>().AddAsync(Titulo);

                // 10. Parcelas
                var parcelas = documento?.NFe?.infNFe?.pag?.detpag ?? new List<DetPag>();

                var parcelasBanco = new List<Parcela>();

                int numeroParcela = 1;
                int parcelaCartaoCredito = 0;

                foreach (var parcela in parcelas)
                {
                    var meioPagamento = (MeioPagamentoSefaz)Convert.ToInt32(parcela.tPag);

                    Parcela parcelaBanco;

                    if (meioPagamento == MeioPagamentoSefaz.CartaoCredito)
                    {
                        parcelaCartaoCredito++;

                        var (Status, DataVencimento) =
                            BuscarDataVencimentoAndStatusParcela(
                                parcela,
                                (DateTime)Titulo.DataEmissao!,
                                NotaStatus,
                                parcelaCartaoCredito
                            );

                        parcelaBanco = new Parcela()
                        {
                            TituloId = Titulo.Id,
                            Numero = numeroParcela,
                            DataVencimento = DataVencimento,
                            Valor = parcela.vPag,
                            Saldo = Status == StatusParcela.Pago ? parcela.vPag : 0,
                            Status = Status,
                            DescricaoPagamento = $"{meioPagamento} - {parcelaCartaoCredito}"
                        };
                    }
                    else
                    {
                        parcelaBanco = new Parcela()
                        {
                            TituloId = Titulo.Id,
                            Numero = numeroParcela,
                            DataVencimento = DocumentoFiscal.DataEmissao,
                            Valor = parcela.vPag,
                            Saldo = parcela.vPag,
                            Status = StatusParcela.Pago,
                            DescricaoPagamento = meioPagamento.ToString()
                        };
                    }

                    parcelasBanco.Add(parcelaBanco);
                    numeroParcela++;
                }

                await _unitOfWork.Repository<Parcela>().AddRangeAsync(parcelasBanco);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion
        }
        #endregion

        #region Private Methods
        private async Task<Guid> BuscarEmpresaIdPorUsarioId(string UsuarioId)
        {
            #region Fields
            #endregion

            #region Logic
            try
            {
                // Buscando a EmpresaId pelo usuario autenticado
                return await _unitOfWork.Repository<UsuarioEmpresa>()
                    .QueryableObject()
                    .Where(x => x.UsuarioId.ToString() == UsuarioId)
                    .Select(x => x.EmpresaId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion
        }
        private StatusTitulo ClassificarStatusTitulo(NfeProc documento, string NotaStatus)
        {
            #region Fields
            List<DetPag>? Pagamentos = documento?.NFe?.infNFe?.pag?.detpag;

            if (Pagamentos == null || !Pagamentos.Any())
                return StatusTitulo.Aberto;

            List<MeioPagamentoSefaz> MeiosAbertos = new List<MeioPagamentoSefaz> {
                MeioPagamentoSefaz.Cheque,
                MeioPagamentoSefaz.BoletoBancario,
                MeioPagamentoSefaz.SemPagamento
            };

            List<MeioPagamentoSefaz> MeiosAVista = new List<MeioPagamentoSefaz> {
                MeioPagamentoSefaz.Dinheiro,
                MeioPagamentoSefaz.Pix,
                MeioPagamentoSefaz.CartaoDebito,
                MeioPagamentoSefaz.TransferenciaBancaria,
            };
            #endregion

            #region Logic
            try
            {
                if (NotaStatus == "C")
                    return StatusTitulo.Cancelado;

                var meiosConvertidos = Pagamentos
                    .Select(x =>
                    {
                        if (int.TryParse(x.tPag, out int tPagInt) &&
                            Enum.IsDefined(typeof(MeioPagamentoSefaz), tPagInt))
                        {
                            return (MeioPagamentoSefaz)tPagInt;
                        }
                        return (MeioPagamentoSefaz?)null;
                    })
                    .Where(x => x.HasValue)
                    .Select(x => x.Value)
                    .ToList();

                bool temAberto = false;
                bool temQuitado = false;

                foreach (var meio in meiosConvertidos)
                {
                    // 🔴 meios já conhecidos como "a prazo"
                    if (MeiosAbertos.Contains(meio))
                    {
                        temAberto = true;
                        continue;
                    }

                    // 🔴 cartão de crédito (regra especial)
                    if (meio == MeioPagamentoSefaz.CartaoCredito)
                    {
                        var NumeroparcelasCartao = Pagamentos
                            .Where(x => x.tPag == "3")
                            .Count();

                        // Se tiver mais de um pagamento → assume parcelado
                        if (NumeroparcelasCartao > 1)
                            temAberto = true;
                        else
                            temQuitado = true;

                        continue;
                    }

                    // 🔴 meios à vista
                    if (MeiosAVista.Contains(meio))
                    {
                        temQuitado = true;
                        continue;
                    }
                }

                // 🔥 decisão final
                if (temAberto && temQuitado)
                    return StatusTitulo.Parcial;

                if (temAberto)
                    return StatusTitulo.Aberto;

                if (temQuitado)
                    return StatusTitulo.Quitado;

                return StatusTitulo.Aberto;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion
        }

        private (StatusParcela Status, DateTime DataVencimento) BuscarDataVencimentoAndStatusParcela(
            DetPag parcela,
            DateTime dataEmissao,
            string? notaStatus,
            int numeroParcela
        )
        {
            try
            {
                var dataVencimento = numeroParcela == 1
                    ? dataEmissao
                    : dataEmissao.AddMonths(numeroParcela);

                // Nota cancelada tem prioridade sobre qualquer regra
                if (notaStatus == "C")
                    return (StatusParcela.Cancelado, dataVencimento);

                // Primeira parcela (cartão crédito normalmente já paga)
                if (numeroParcela == 1)
                    return (StatusParcela.Pago, dataVencimento);

                // Demais parcelas ficam em aberto
                return (StatusParcela.Aberto, dataVencimento);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private async Task<Guid> BuscarCategoriaIdPorCfop(string Cfop)
        {
            #region Fields
            var repoCategoriaFinanceira = _unitOfWork.Repository<CategoriaFinanceira>().QueryableObject();
            #endregion

            #region Logic
            try
            {
                return await repoCategoriaFinanceira
                    .Where(x => x.Cfop == Cfop)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion

        }
        #endregion
    }
}
