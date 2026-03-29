using System.ComponentModel;

namespace rh.financeiro.Domain.Enums
{
    /// <summary>
    /// Enum com códigos de status de sucesso e erros
    /// </summary>
    public enum ReturnStatus
    {
        #region INFORMATION RESPONSES

        [Description("Continue")]
        Continue = 100,

        [Description("Processing")]
        Processing = 102,

        [Description("Early Hints")]
        EarlyHints = 103,

        #endregion INFORMATION RESPONSES

        #region SUCCESSFUL RESPONSES

        /// <summary>
        /// Ok
        /// </summary>
        [Description("Ok")]
        Ok = 200,

        [Description("Created")]
        Created = 201,

        [Description("Accepted")]
        Accepted = 202,

        [Description("Non-Authoritative Information")]
        NonAuthoritativeInformation = 203,

        [Description("No Content")]
        NoContent = 204,

        [Description("Reset Content")]
        ResetContent = 205,

        [Description("Partial Content")]
        PartialContent = 206,

        [Description("Multi-Status ")]
        MultiStatus = 207,

        [Description("IM Used")]
        IMUsed = 226,

        #endregion SUCCESSFUL RESPONSES

        #region REDIRECTS

        [Description("Multiple Choice")]
        MultipleChoice = 300,

        [Description("Moved Permanently")]
        MovedPermanently = 301,

        [Description("Found")]
        Found = 302,

        [Description("See Other")]
        SeeOther = 303,

        [Description("Not Modified")]
        NotModified = 304,

        [Description("Use Proxy")]
        UseProxy = 305,

        [Description("Unused")]
        Unused = 306,

        [Description("Temporary Redirect")]
        TemporaryRedirect = 307,

        [Description("Permanent Redirect")]
        PermanentRedirect = 308,

        #endregion REDIRECTS

        #region CLIENT ERRORS

        /// <summary>
        /// Quando request enviado esta errado
        /// </summary>
        [Description("Bad Request")]
        BadRequest = 400,

        [Description("Unauthorized")]
        Unauthorized = 401,

        [Description("Payment Required")]
        PaymentRequired = 402,

        [Description("Forbidden")]
        Forbidden = 403,

        [Description("Not Found")]
        NotFound = 404,

        [Description("Method Not Allowed")]
        MethodNotAllowed = 405,

        [Description("Not Acceptable")]
        NotAcceptable = 406,

        [Description("Proxy Authentication Required")]
        ProxyAuthenticationRequired = 407,

        [Description("Request Timeout")]
        RequestTimeout = 408,

        [Description("Conflict")]
        Conflict = 409,

        [Description("Gone")]
        Gone = 410,

        [Description("Length Required")]
        LengthRequired = 411,

        [Description("Precondition Failed")]
        PreconditionFailed = 412,

        [Description("Payload Too Large")]
        PayloadTooLarge = 413,

        [Description("URI Too Long")]
        URITooLong = 414,

        [Description("Unsupported Media Type")]
        UnsupportedMediaType = 415,

        [Description("Requested Range Not Satisfiable")]
        RequestedRangeNotSatisfiable = 416,

        [Description("Expectation Failed")]
        ExpectationFailed = 417,

        [Description("I'm a teapot")]
        ImATeapot = 418,

        [Description("Misdirected Request")]
        MisdirectedRequest = 421,

        [Description("Unprocessable Entity")]
        UnprocessableEntity = 422,

        [Description("Locked")]
        Locked = 423,

        [Description("Failed Dependency")]
        FailedDependency = 424,

        [Description("Too Early")]
        TooEarly = 425,

        [Description("Upgrade Required")]
        UpgradeRequired = 426,

        [Description("Precondition Required")]
        PreconditionRequired = 428,

        [Description("Too Many Requests")]
        TooManyRequests = 429,

        [Description("Request Header Fields Too Large")]
        RequestHeaderFieldsTooLarge = 431,

        [Description("Unavailable For Legal Reasons")]
        UnavailableForLegalReasons = 451,

        #endregion CLIENT ERRORS

        #region SERVER ERRORS

        [Description("Internal Server Error")]
        InternalServerError = 500,

        [Description("Not Implemented")]
        NotImplemented = 501,

        [Description("Bad Gateway")]
        BadGateway = 502,

        [Description("Service Unavailable")]
        ServiceUnavailable = 503,

        [Description("Gateway Timeout")]
        GatewayTimeout = 504,

        [Description("HTTP Version Not Supported")]
        HTTPVersionNotSupported = 505,

        [Description("Variant Also Negotiates")]
        VariantAlsoNegotiates = 506,

        [Description("Insufficient Storage")]
        InsufficientStorage = 507,

        [Description("Loop Detected")]
        LoopDetected = 508,

        [Description("Not Extended")]
        NotExtended = 510,

        [Description("Network Authentication Required")]
        NetworkAuthenticationRequired = 511

        #endregion SERVER ERRORS
    }
}
