using Microsoft.Extensions.Configuration;
using rh.financeiro.Domain.Common.DocumentosNfes;
using rh.financeiro.Domain.Dto.Response.Nfe.NotasFiscais;
using rh.financeiro.Domain.Dto.Response.Nfe.Token;
using rh.financeiro.Domain.Interfaces.Service.Nfe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace rh.financeiro.Services.Services.Nfe
{
    public class NfeService : INfeService
    {
        #region Fields
        private readonly IConfiguration _configuration;
        private readonly string _BaseUrl;
        public HttpClient _httpClient = new();

        #endregion

        #region CONS
        public NfeService(IConfiguration configuration)
        {
            _configuration = configuration;
            _BaseUrl = configuration["Nfe:Maino:BaseUrl"]!;
        }
        #endregion

        #region Public Methods
        public async Task<List<NotaFiscal>> BuscarDocumentosEmitidos(DateTime inicio, DateTime fim, string Cnpj)
        {
            #region Fields
            string? Token = await GetToken(Cnpj);
            string DataInicio = inicio.ToString("yyyy-MM-dd");
            string DataFim = fim.ToString("yyyy-MM-dd");
            List<NotaFiscal> Nfes = [];
            #endregion

            #region Logic
            try
            {
                // Realizando a primeira chamada. Buscando os primeiros 25 -> Page 1
                var Primeirosdocumentos = await BuscarNotasFiscaisPorPeriodo(DataInicio, DataFim, Token, 1);

                // Calculando os pages
                int pages = (int)Math.Ceiling(Primeirosdocumentos.Pagination.Total / 25.0);

                // Antes de ir para proxima pagina, salvar os documentos da page 1
                var PrimeirasnotasFiscais = Primeirosdocumentos.NotasFiscais;
                Nfes.AddRange(PrimeirasnotasFiscais);

                // O numero de paginas deve conter mais de uma pagina, para seguir para o proximo!
                if(pages > 1)
                {
                    int i = 2; // Comeca com a segunda pagina
                    while (i <= pages)
                    {
                        // Buscando da proxima pagina
                        var proximosDocumentos = await BuscarNotasFiscaisPorPeriodo(DataInicio, DataFim, Token, i);

                        // Salvando na lista, apenas as notas fiscais
                        var ProximasNotasFiscais = proximosDocumentos.NotasFiscais;
                        Nfes.AddRange(ProximasNotasFiscais);

                        // Incremetando o contador
                        i++;
                    }
                }
                return Nfes;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion
        }
        public async Task<List<NotaFiscal>> BuscarDocumentoPorNumeroNfes(List<string> NumerosNfe, string Cnpj)
        {
            #region Fields
            string? Token = await GetToken(Cnpj);
            List<NotaFiscal> Nfes = [];
            #endregion

            #region Logic
            try
            {
                #region Fields
                var Url = $"{_BaseUrl}/notas_fiscais_emitidas";
                #endregion

                #region Logic
                try
                {
                    foreach(var numeroNfe in NumerosNfe)
                    {
                        // Limpando o header e inserindo o token
                        _httpClient.DefaultRequestHeaders.Clear();
                        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                        // Configurando o envio
                        var response = await _httpClient.GetAsync($"{Url}?numero_nfe={numeroNfe}&exibir_xmls=true");
                        var responseJson = await response.Content.ReadAsStringAsync();

                        var result = JsonSerializer.Deserialize<NotaFiscal>(responseJson);
                        Nfes.Add(result);
                    }

                    return Nfes;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
                #endregion
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
        private async Task<string?> GetToken(string Cnpj)
        {
            #region Fields
            string Email = _configuration["Nfe:Maino:Email"]!;
            string Senha = _configuration["Nfe:Maino:Senha"]!;
            string ApplicationUid = _configuration["Nfe:Maino:Application_Uid"]!;
            #endregion

            #region Logic
            try
            {
                var url = $"{_BaseUrl}/authentication";

                var body = new
                {
                    email = Email,
                    password = Senha,
                    application_uid = ApplicationUid
                };
                // Configurando o body
                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                // Recebendo a resposta
                var response = await _httpClient.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                // Deserializando
                var result = JsonSerializer.Deserialize<TokenResponse>(responseString);

                // Retornando o token da Empresa correspodente
                return result?[Cnpj].AccessToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion
        }

        private async Task<NotasFiscaisResponse> BuscarNotasFiscaisPorPeriodo(string inicio, string fim, string Token,int page)
        {
            #region Fields
            var Url = $"{_BaseUrl}/notas_fiscais_emitidas";
            #endregion

            #region Logic
            try
            {
                // Limpando o header e inserindo o token
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                // Configurando o envio
                var response = await _httpClient.GetAsync($"{Url}?data_inicio={inicio}&data_fim={fim}&exibir_xmls=true&page={page}");
                var responseJson = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<NotasFiscaisResponse>(responseJson);
                return result;
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
