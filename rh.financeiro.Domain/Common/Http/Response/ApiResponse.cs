using System.Text.Json.Serialization;
using rh.financeiro.CrossCuting;
using rh.financeiro.Domain.Enums;

namespace rh.financeiro.Domain.Common.Http.Response
{
    public class ResponseApi
    {
        #region METHODS

        public static ResponseApi FormatResponse(ReturnStatus returnStatus, ReturnCodes returnCodes, bool success)
        {
            return new()
            {
                Code = (int)returnCodes,
                Data = new { },
                Message = Utils.GetEnumDescription(returnCodes),
                Status = (int)returnStatus,
                Success = success,
            };
        }

        public static ResponseApi FormatResponse(ReturnStatus returnStatus, ReturnCodes returnCodes, bool success, object data)
        {
            return new()
            {
                Code = (int)returnCodes,
                Data = data,
                Message = Utils.GetEnumDescription(returnCodes),
                Status = (int)returnStatus,
                Success = success,
            };
        }

        public static ResponseApi FormatResponse(ReturnStatus returnStatus, ReturnCodes returnCodes, bool success, string? mensagem)
        {
            return new()
            {
                Code = (int)returnCodes,
                Data = new { },
                Message = mensagem,
                Status = (int)returnStatus,
                Success = success,
            };
        }

        public static ResponseApi FormatResponse(ReturnStatus returnStatus, ReturnCodes returnCodes, bool success, string? mensagem, object data)
        {
            return new()
            {
                Code = (int)returnCodes,
                Data = data,
                Message = mensagem,
                Status = (int)returnStatus,
                Success = success,
            };
        }

        public static ResponseApi FormatResponse(Exception ex)
        {
            return new()
            {
                Message =
$@"MESSAGE => {ex.Message}.
INNER EXCEPTION => {ex.InnerException}.
STACK TRACE =>{ex.StackTrace}.",
                Code = (int)ReturnCodes.InternalError,
                Status = (int)ReturnStatus.InternalServerError,
                Success = false,
            };
        }

        public static ResponseApi FormatResponse(ResponseApi responseApi, object data)
        {
            responseApi.Data = data;
            return responseApi;
        }

        #endregion METHODS

        #region PROPERTIES

        [JsonInclude()]
        [JsonPropertyName("code")]
        [JsonPropertyOrder(1)]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public int Code { get; set; }

        [JsonInclude()]
        [JsonPropertyName("data")]
        [JsonPropertyOrder(4)]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public object Data { get; set; } = default!;

        [JsonInclude()]
        [JsonPropertyName("message")]
        [JsonPropertyOrder(3)]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public string Message { get; set; } = default!;

        [JsonInclude()]
        [JsonPropertyName("status")]
        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public int Status { get; set; }
            
        [JsonInclude()]
        [JsonPropertyName("success")]
        [JsonPropertyOrder(0)]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public bool Success { get; set; }

        #endregion PROPERTIES
    }
}
