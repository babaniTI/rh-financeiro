using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace rh.financeiro.CrossCuting
{
    public static class Utils
    {
        private static Random _random = new Random();

        #region PUBLIC_METHODS

        public static string GetEnumDescription(Enum value)
        {
            return value.GetType().GetMember(value.ToString()).FirstOrDefault()?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? value.ToString();
        }

        public static string ConverterXmlParaJson(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            string json = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, true);

            return json;
        }

        public static string NormalizarTelefone(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return telefone;

            // Remove espaços, traços, parênteses e pontos
            var digits = new string(telefone.Where(char.IsDigit).ToArray());

            // Se começar com 0, remove
            if (digits.StartsWith("0"))
                digits = digits.TrimStart('0');

            // Se não começar com DDI (+55 para Brasil), adiciona
            if (!digits.StartsWith("55"))
                digits = "55" + digits;

            return digits;
        }

        public static string OnlyNumbers(this string value)
        {
            return Regex.Replace(value, @"[^\d]", "");
        }

        public static bool IsCpf(string? cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            cpf = new string(cpf.Where(char.IsDigit).ToArray());
            return cpf.Length == 11;
        }

        public static bool IsCnpj(string? cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            cnpj = new string(cnpj.Where(char.IsDigit).ToArray());
            return cnpj.Length == 14;
        }

        public static string EncryptCIS(this string value)
        {
            string output = null;
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    int DecodeKey;
                    char[] aux;
                    for (int i = 0; i < value.Length; i++)
                    {
                        DecodeKey = (i + 1) % 26;
                        aux = value.Substring(i, 1).ToCharArray();
                        output += (char)(DecodeKey + (int)aux[0]);
                    }
                }
                catch { }
            }
            return output;
        }
        public static string FormatXmlRequest<T>(T value, XmlSerializerNamespaces xmlSerializerNamespaces)
        {
            XmlSerializer xmlSerializer = new(typeof(T));
            XmlWriterSettings xmlWriterSettings = new()
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = true,
            };
            StringBuilder stringBuilder = new();
            using XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSettings);
            xmlSerializer.Serialize(xmlWriter, value, xmlSerializerNamespaces);

            return stringBuilder.ToString();
        }

        public static T FormatXmlResponse<T>(string value)
        {
            XmlSerializer serializer = new(typeof(T));
            using StringReader reader = new(value);
            return (T)serializer.Deserialize(reader)!;
        }

        public static string DecryptCIS(this string value)
        {
            string output = null;
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    int DecodeKey;
                    char[] aux;
                    for (int i = 0; i < value.Length; i++)
                    {
                        DecodeKey = (i + 1) % 26;
                        aux = value.Substring(i, 1).ToCharArray();
                        output += (char)((int)aux[0] - DecodeKey);
                    }
                }
                catch { }
            }
            return output;
        }

        public static string? GetUserId(HttpContext httpContext)
        {
            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;

            var token = authHeader.Replace("Bearer ", "").Trim();
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userId = jwtToken.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type.Equals("nameid", StringComparison.OrdinalIgnoreCase)
            )?.Value;

            return userId;
        }


        public static string? GetSessionID(HttpContext httpContext)
        {
            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;

            var token = authHeader.Replace("Bearer ", "").Trim();
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var sessionId = jwtToken.Claims.FirstOrDefault(c =>
                c.Type.Equals("SessionID", StringComparison.OrdinalIgnoreCase) ||
                c.Type.Equals("sid", StringComparison.OrdinalIgnoreCase)
            )?.Value;

            return sessionId;
        }


        public static string? GetClientIp(HttpContext httpContext)
        {
            var ip = httpContext.Connection.RemoteIpAddress;

            // Se for IPv6 mapeado para IPv4, converte
            if (ip != null && ip.IsIPv4MappedToIPv6)
                ip = ip.MapToIPv4();

            return ip?.ToString();
        }

        public static List<ValidationResult> ValidateObject<T>(T obj)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(obj, null, null);
            Validator.TryValidateObject(obj, context, results, validateAllProperties: true);
            return results;
        }

        public static string MaskAll(string value)
        {
            return new string('*', value.Length);
        }

        public static string MaskCPF(string cpf)
        {
            if (cpf.Length <= 5) return new string('*', cpf.Length);
            return cpf.Substring(0, 3) + new string('*', cpf.Length - 5) + cpf[^2..];
        }


        public static string MaskNome(string nome)
        {
            if (nome.Length <= 4)
            {
                // Mascarar caracteres do meio se o comprimento for menor ou igual a 4
                int maskLength = nome.Length - 2;
                return nome.Substring(0, 1) + new string('*', maskLength) + nome.Substring(nome.Length - 1);
            }
            else
            {
                // Manter os três primeiros caracteres e os três últimos intactos
                return nome.Substring(0, 3) + new string('*', nome.Length - 6) + nome.Substring(nome.Length - 3);
            }
        }

        public static string MaskEmail(string email)
        {
            // Divide o email em duas partes: local e domínio
            var parts = email.Split('@');

            // Verifica se não há exatamente duas partes após a divisão (local e domínio)
            if (parts.Length != 2)

                // Se não, retorna uma string com asteriscos (*) do mesmo comprimento do email
                return new string('*', email.Length);

            // Obtém o nome local e o domínio do email
            var local = parts[0];
            var domain = parts[1];

            if (local.Length < 3)
            {
                return new string('*', local.Length) + "@" + domain;
            }

            // Verifica se o comprimento do nome local é menor ou igual a 6
            if (local.Length <= 6)
            {
                // Se for, retorna uma string com asteriscos (*) do mesmo comprimento do email
                return local.Substring(0, 3) + new string('*', local.Length - 3) + "@" + domain;
            }

            // Mantém os três primeiros caracteres do nome local
            var visiblePart = local.Substring(0, 3);

            // Calcula o número de caracteres a serem mascarados
            int maskLength = local.Length - 6;

            // Retorna a parte visível do nome local seguida pelos asteriscos, o último caractere visível, o "@" e o domínio
            return visiblePart + new string('*', maskLength) + local.Substring(local.Length - 3) + "@" + domain;
        }

        public static string MaskTelefone(string telefone)
        {
            if (telefone.Length <= 2)
                return telefone; // Se o telefone tiver 2 dígitos ou menos, retorna como está

            if (telefone.Length <= 6)
                return telefone.Substring(0, 2) + new string('*', telefone.Length - 2); // Se o telefone tiver entre 3 e 6 dígitos, mostra apenas os dois primeiros

            return telefone.Substring(0, 2) + new string('*', telefone.Length - 6) + telefone.Substring(telefone.Length - 4); // Se o telefone tiver mais de 6 dígitos, mostra os dois primeiros e os últimos quatro
        }

        public static string MaskCartaoNumero(string numero)
        {
            // Verifica se o comprimento do número do cartão é menor ou igual a 10
            if (numero.Length <= 10)

                // Se for, retorna uma string com asteriscos (*) do mesmo comprimento do número do cartão
                return new string('*', numero.Length);

            // Calcula quantos dígitos devem ser mascarados no meio
            int maskLength = numero.Length - 10;

            // Retorna os seis primeiros dígitos visíveis, os dígitos do meio mascarados e os quatro últimos visíveis
            return numero.Substring(0, 6) + new string('*', maskLength) + numero.Substring(numero.Length - 4);
        }

        public static string ObterIpLocal()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return null; // Ou lançar uma exceção, se preferir
        }

        public static string GenerateToken(string tipo)
        {
            string InitialLetters = tipo switch
            {
                "P" => "PD",
                "V" => "VC"
            };

            // Gerar um número aleatório (pode ser ajustado para o tamanho desejado)
            int randomNumber = _random.Next(10000000, 99999999); // Número entre 10000000 e 99999999

            // Gerar uma sequência aleatória de letras e números (TUVU8O, por exemplo)
            string randomString = GenerateRandomString(6); // Se você quiser 6 caracteres aleatórios

            // Montar o token
            string token = $"{InitialLetters}-{randomNumber}-{randomString}";

            return token;
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] stringChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[_random.Next(chars.Length)];
            }
            return new string(stringChars);
        }

        #endregion PUBLIC_METHODS
    }



    
}
