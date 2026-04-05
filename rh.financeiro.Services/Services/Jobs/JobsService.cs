using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using rh.financeiro.CrossCuting;
using rh.financeiro.Data.Context.Default;
using rh.financeiro.Domain.Common.DocumentosNfes;
using rh.financeiro.Domain.Dto.Request.Jobs.ImportarDocumentosAndGerarTitulos;
using rh.financeiro.Domain.Dto.Response.Nfe.NotasFiscais;
using rh.financeiro.Domain.Entities;
using rh.financeiro.Domain.Enums;
using rh.financeiro.Domain.Interfaces.Service.Jobs;
using rh.financeiro.Domain.Interfaces.Service.Nfe;
using rh.financeiro.Domain.Interfaces.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace rh.financeiro.Services.Services.Jobs
{
    public class JobsService : IJobsService
    {
        #region Fields
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork<DefaultContext> _unitOfWork;
        private readonly INfeService _nfeService;
        #endregion

        #region CONS
        public JobsService(IUnitOfWork<DefaultContext> unitOfWork, IConfiguration configuration, INfeService nfeService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _nfeService = nfeService;
        }
        #endregion

        #region Public Methods
        public async Task<(bool success, string Message)> ImportarDocumentosAndGerarTitulos(ImportarDocumentosAndGerarTitulosRequest request)
        {
            #region Fields
            DateTime? DataInicio = null;
            DateTime? DataFim = null;
            #endregion

            #region Consistencias

            var EmpresaId = await BuscarEmpresaIdPorCnpj(request.cnpj);

            // Definindo o periodo da busca 
            if (!string.IsNullOrEmpty(request.inicio))
                DataInicio = Convert.ToDateTime(request.inicio);

            if (!string.IsNullOrEmpty(request.fim))
                DataFim = Convert.ToDateTime(request.fim);

            // Se nao vier, pegar o ultimo documento importado
            if(DataInicio == null)
                DataInicio = await BuscarDataUltimoDocumentoImportadoPorEmpresa(EmpresaId);

            if (DataFim == null)
                DataFim = DateTime.Now;
            #endregion

            #region Logic
            try
            {
                // Buscar as Notas emitidas no periodo informado, e respeitar o cnpj. Buscar apenas que tem status Aceita
                var NotasFiscais = await _nfeService.BuscarDocumentosEmitidos((DateTime)DataInicio, (DateTime)DataFim, request.cnpj);

                // Processar cada xml, e colocar isso em uma Lista
                var xmls = NotasFiscais.Where(x => x.Xml != null).Select(x => x.Xml).ToList();
                var documentos = BuscarNotasDetalhesPorXmls(xmls);

                foreach(var documento in documentos)
                {
                    Participante? participante = null;
                    List<DocumentoFiscalItem> documentoFiscalItems = [];
                    List<TributoItem> TributoItems = [];
                    List<Parcela> parcelasBanco = [];
                    Guid CategoriaFinanceiraId = await BuscarCategoriaFinanceiraReceita();

                    // Verificar se já existe na base da empresa. Se existir, vai pra o proximo
                    string? DocumentoChaveAcesso = documento?.NFe?.infNFe?.Id?.Replace("NFe", "");
                    bool jaExiste = await DocumentoJaExistePorChaveAndEmpresa(DocumentoChaveAcesso, EmpresaId);

                    if (jaExiste)
                        continue;

                    // Buscando o status da Nota, que vem do ERP Maino
                    var NotaStatus = NotasFiscais
                        .Where(x => x.ChaveAcessoNfe == DocumentoChaveAcesso)
                        .Select(x => x.Status)
                        .FirstOrDefault();

                    // Caso tenha informacoes do consumidor, cria um participante
                    if (documento?.NFe?.infNFe?.dest != null)
                    {
                        participante = new Participante()
                        {
                            EmpresaId = EmpresaId,
                            Tipo = TipoParticipante.Cliente,
                            Nome = documento?.NFe?.infNFe?.dest?.xNome,
                            Documento = documento?.NFe?.infNFe?.dest?.CPF ?? documento?.NFe?.infNFe?.dest?.CNPJ,
                        };

                        await _unitOfWork.Repository<Participante>().AddAsync(participante);
                    }

                    // 1. Documento_fiscal
                    var DocumentoFiscal = new DocumentoFiscal()
                    {
                        EmpresaId = EmpresaId,
                        Tipo = TipoDocumentoFiscal.NFCE,
                        Numero = documento?.NFe?.infNFe?.ide?.nNF,
                        Serie = documento?.NFe?.infNFe?.ide?.serie,
                        Chave = DocumentoChaveAcesso,
                        ParticipanteId = participante != null ? participante.Id : null,
                        DataEmissao = documento?.NFe?.infNFe?.ide?.dhEmi,
                        ValorTotal = documento?.NFe?.infNFe?.det?.Sum(x => x.prod?.vProd),
                        DadosTributarios = JsonSerializer.Serialize(documento),
                        status = NotaStatus switch
                        {
                            "A" => TipoStatusDocumentoFiscal.Processado,
                            "C" => TipoStatusDocumentoFiscal.Cancelado,
                            "ET" => TipoStatusDocumentoFiscal.Pendente,
                            _ => TipoStatusDocumentoFiscal.Pendente
                        }
                    };

                    await _unitOfWork.Repository<DocumentoFiscal>().AddAsync(DocumentoFiscal);

                    //    2. Documento_item
                    var itens = documento?.NFe?.infNFe?.det;
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

                    // 3. tributo_item
                    foreach (var item in itens)
                    {
                        // Achando o DocumentoItemId
                        var documentoItem_id = documentoFiscalItems.Where(x =>
                            x.ProdutoCodigo == item?.prod?.cProd &&
                            x.Quantidade == item?.prod?.qCom &&
                            x.ValorUnitario == (item?.prod?.vProd / item?.prod?.qCom) &&
                            x.ValorTotal == item?.prod?.vProd
                        ).Select(x => x.Id).FirstOrDefault();

                        var TributoItemCBS = new TributoItem()
                        {
                            ItemId = documentoItem_id,
                            Tipo = TipoTributo.CBS,
                            Base = item?.imposto?.IBSCBS?.gIBSCBS?.vBC,
                            Aliquota = item?.imposto?.IBSCBS?.gIBSCBS?.gCBS?.pCBS,
                            Valor = item?.imposto?.IBSCBS?.gIBSCBS?.gCBS?.vCBS,
                        };

                        var TributoItemIBS = new TributoItem()
                        {
                            ItemId = documentoItem_id,
                            Tipo = TipoTributo.IBS,
                            Base = item?.imposto?.IBSCBS?.gIBSCBS?.vBC,
                            Aliquota = item?.imposto?.IBSCBS?.gIBSCBS?.GIBSMun?.pIBSMun + item?.imposto?.IBSCBS?.gIBSCBS?.gIBSUF?.pIBSUF, // Soma do Municipal com o UF (Estadual),
                            Valor = item?.imposto?.IBSCBS?.gIBSCBS?.GIBSMun?.vIBSMun + item?.imposto?.IBSCBS?.gIBSCBS?.gIBSUF?.vIBSUF, // Soma do Municipal com o UF (Estadual),
                        };

                        TributoItems.Add(TributoItemCBS);
                        TributoItems.Add(TributoItemIBS);
                    }

                    await _unitOfWork.Repository<TributoItem>().AddRangeAsync(TributoItems);

                    //    4. Titulo
                    var Titulo = new Titulo()
                    {
                        EmpresaId = EmpresaId,
                        Tipo = TipoTitulo.RECEBER,
                        DocumentoId = DocumentoFiscal.Id,
                        ParticipanteId = DocumentoFiscal.ParticipanteId,
                        CategoriaId = CategoriaFinanceiraId,
                        ValorTotal = (Decimal)DocumentoFiscal.ValorTotal!,
                        DataEmissao = DocumentoFiscal.DataEmissao,
                        Status = ClassificarStatusTitulo(documento,NotaStatus)
                    };
                    await _unitOfWork.Repository<Titulo>().AddAsync(Titulo);

                    // 5. Parcela
                    var parcelas = documento?.NFe?.infNFe?.pag?.detpag;
                    int numeroparcela = 1;
                    foreach (var parcela in parcelas)
                    {
                        Parcela parcelaBanco = new();
                        var MeioPagamento = (MeioPagamentoSefaz)Convert.ToInt32(parcela.tPag);
                        int parcelaCartaoCredito = 0; // incrementador de parcela de cartao de credito!

                        if (MeioPagamento == MeioPagamentoSefaz.CartaoCredito)
                        {
                            // Toda vez que entra aqui, ele incrementa
                            parcelaCartaoCredito++;

                            (StatusParcela Status, DateTime DataVencimento) = BuscarDataVencimentoAndStatusParcela(parcela, (DateTime)Titulo.DataEmissao!,NotaStatus, parcelaCartaoCredito);
                            parcelaBanco = new Parcela()
                            {
                                TituloId = Titulo.Id,
                                Numero = numeroparcela,
                                DataVencimento = DataVencimento,
                                Valor = parcela.vPag,
                                Saldo = Status == StatusParcela.Pago ? parcela.vPag : 0,
                                Status = Status,
                                DescricaoPagamento = $"{MeioPagamento.ToString()} - {parcelaCartaoCredito}"
                            };
                        }
                        else
                        {
                            parcelaBanco = new Parcela()
                            {
                                TituloId = Titulo.Id,
                                Numero = numeroparcela,
                                DataVencimento = DocumentoFiscal.DataEmissao,
                                Valor = parcela.vPag,
                                Saldo = parcela.vPag,
                                Status = StatusParcela.Pago,
                                DescricaoPagamento = $"{MeioPagamento.ToString()}"
                            };
                        }

                        parcelasBanco.Add(parcelaBanco);
                        numeroparcela++;
                    }

                    await _unitOfWork.Repository<Parcela>().AddRangeAsync(parcelasBanco);
                }

                // Se deu tudo certo, retorna true com a mensagem de sucess - "Importacao concluida com sucesso!"
                return (true, "Importacao concluida com sucesso!");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion
        }

        public async Task<(bool success, string Message)> AtualizarDocumentosAndTitulos(string Cnpj)
        {
            #region Fields
            var repoDocumentoFiscal = _unitOfWork.Repository<DocumentoFiscal>().QueryableObject();
            var repoTitulo = _unitOfWork.Repository<Titulo>().QueryableObject();
            var repoParcela = _unitOfWork.Repository<Parcela>().QueryableObject();
            var EmpresaId = await BuscarEmpresaIdPorCnpj(Cnpj);
            Guid CategoriaFinanceiraId = await BuscarCategoriaFinanceiraReceita();
            List<Parcela> parcelasBanco = [];
            #endregion

            #region Logic
            try
            {
                // Buscar da base, todos os documentos fiscais que tem o status de Processado e Pendente. Caso tenha, buscar os seus titulos e as parcelas correspodentes
                var DocumentosXTitulosXParcelas = await (
                    from df in repoDocumentoFiscal
                    join t in repoTitulo
                        on df.Id equals t.DocumentoId
                    join p in repoParcela
                        on t.Id equals p.TituloId
                    where 
                        df.EmpresaId == EmpresaId &&
                        df.status == TipoStatusDocumentoFiscal.Processado
                    group p by new
                    {
                        Documento = df,
                        Titulo = t
                    } into g
                    select new
                    {
                        Documento = g.Key.Documento,
                        Titulo = g.Key.Titulo,
                        Parcelas = g.ToList()
                    }
                ).ToListAsync();

                // Ir no maino, pegar todos esses documentos pela NumeroNfe
                List<string> NumerosNfes = DocumentosXTitulosXParcelas.Select(x => x.Documento.Numero).ToList();
                var NotasFiscais = await _nfeService.BuscarDocumentoPorNumeroNfes(NumerosNfes,Cnpj);

                // Agora, vamos atualizar
                foreach(var documento in DocumentosXTitulosXParcelas)
                {
                    // Setando as entidades globais
                    var documentoFiscal = documento.Documento;
                    var Titulo = documento.Titulo;
                    var parcelas = documento.Parcelas;

                    // Buscando a nota do documento
                    var NotaFiscal = NotasFiscais
                        .Where(x => x.NumeroNfe.ToString() == documento.Documento.Numero)
                        .FirstOrDefault();

                    // 1. Se o status da nota for C, atualiza para Cancelado os documentos, titulos e as parcelas
                    if (NotaFiscal?.Status == "C")
                    {
                        // Cancelando o documento, titulo e as parcelas
                        documentoFiscal.status = TipoStatusDocumentoFiscal.Cancelado;
                        Titulo.Status = StatusTitulo.Cancelado;

                        // Cancelando a parcela, e zerando os valores dos que ja foram pago, pois os valores vao ser estornados posteriormente
                        parcelas.ForEach(x =>
                        {
                            x.Status = StatusParcela.Cancelado;
                            x.Saldo = 0m;
                        });
                    }
                }

                return (true, "Atualizacão concluida com sucesso");
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
        private async Task<Guid> BuscarEmpresaIdPorCnpj(string Cnpj)
        {
            #region Fields
            var repoEmpresa = _unitOfWork.Repository<Empresa>().QueryableObject();
            #endregion

            #region Logic
            return await repoEmpresa
                .Where(x => x.Cnpj == Cnpj)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
            #endregion
        }

        private async Task<DateTime?> BuscarDataUltimoDocumentoImportadoPorEmpresa(Guid EmpresaId)
        {
            #region Fields
            var repoDocumento = _unitOfWork.Repository<DocumentoFiscal>().QueryableObject();
            #endregion

            #region Logic
            return await repoDocumento
                    .Where(x => x.EmpresaId == EmpresaId)
                    .OrderByDescending(x => x.DataEmissao)
                    .Select(x => x.DataEmissao)
                    .FirstOrDefaultAsync();
            #endregion
        }

        private List<NfeProc> BuscarNotasDetalhesPorXmls(List<string> xmls)
        {
            #region Fields
            List<NfeProc> DocumentosNfesDetalhes = [];
            #endregion

            #region Logic
            try
            {
                // Percorrendo os xmls
                foreach(var xml in xmls)
                {
                    // Converterndo o xml para Json
                    var json = Utils.ConverterXmlParaJson(xml);

                    // Deserializando para o Objeto
                    var NotaDetalhes = JsonSerializer.Deserialize<NfeProc>(json);

                    // Adiciona na lista
                    DocumentosNfesDetalhes.Add(NotaDetalhes);
                }

                return DocumentosNfesDetalhes;

            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion

        }

        private async Task<bool> DocumentoJaExistePorChaveAndEmpresa(string? ChaveAcesso, Guid EmpresaId)
        {
            #region Fields
            var repoDocumentoFiscal = _unitOfWork.Repository<DocumentoFiscal>().QueryableObject();
            #endregion

            #region Logic
            try
            {
                if(string.IsNullOrEmpty(ChaveAcesso))
                    return true; // Nao tem chave, nem gera a nota fiscal!

                return await repoDocumentoFiscal.AnyAsync(x => 
                    x.EmpresaId == EmpresaId &&
                    x.Chave == ChaveAcesso
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion

        }
        #endregion

        private async Task<Guid> BuscarCategoriaFinanceiraReceita()
        {
            #region Fields
            var repoCategoriaFinanceira = _unitOfWork.Repository<CategoriaFinanceira>().QueryableObject();
            #endregion

            #region Logic
            try
            {
                return await repoCategoriaFinanceira
                    .Where(x => x.Tipo == TipoCategoriaFinanceira.Receita)
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
    }
}
