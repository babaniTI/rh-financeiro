using System.ComponentModel;

namespace rh.financeiro.Domain.Enums
{
    /// <summary>
    /// Enum com códigos de sucessos e erros
    /// </summary>
    public enum ReturnCodes
    {
        [Description("Success")]
        Ok = 0,

        [Description("Not Found")]
        NotFound = -1,

        [Description("Invalid values.")]
        InvalidValue = -2,

        [Description("Credenciais inválidas.")]
        InvalidUserCredentials = -3,

        [Description("Sua sessão expirou. Por favor faça o login novamente.")]
        ExpiredToken = -4,

        [Description("Delete operation unsuccessful")]
        DeleteRequestDenied = -5,

        [Description("Create operation unsuccessful")]
        CreateRequestDenied = -6,

        [Description("Update operation unsuccessful")]
        UpdateRequestDenied = -7,

        [Description("Internal error")]
        InternalError = -9,

        [Description("Senha inválida.")]
        InvalidPassword = -10,

        [Description("Usuário inválido.")]
        InvalidUser = -11,

        [Description("Senhas diferentes.")]
        DifferentPasswords = -12,

        [Description("Tipo inválido.")]
        InvalidType = -13,

        [Description("Usuário já existe.")]
        UserExist = -14,

        [Description("Usuário ou senha inválido.")]
        InvalidUserPassword = -15,

        [Description("Empty Request.")]
        EmptyRequest = -16,

        [Description("Exceção do Try-Catch")]
        ExceptionEx = -17,

        [Description("Access Denied")]
        AccessDenied = -18,

        [Description("Aguardando Liberação da administração")]
        UserTempAdd = -19,

        [Description("Sessão do sistema expirou! Por favor entre novamente!")]
        ExpiredTokenSystem = -20,

        [Description("E-mail não enviado!")]
        NotSend = -21,

        [Description("Valores não Aceitos!")]
        NotAcceptable = -22,

        [Description("Senha expirada. Por favor redefina sua senha!")]
        ResetPassword = -23,

        [Description("Operation unsuccessful")]
        OperationUnsuccessful = -24,

        [Description("Object already exists")]
        ObjctAlreadyexists = -25,

        [Description("Cart Finished")]
        CartFinished = -26,

        [Description("File Empty")]
        FileEmpty = -27,

        [Description("Directory Not Exists")]
        DirectoryNotExists = -28,

        [Description("Não foi possível finalizar a transação. Por favor, tente mais tarde.")]
        Instability = -29,

        [Description("Transação recusada.")]
        FailTransaction = -30,

        [Description("Informar o Campo {0}.")]
        EmptyField = -31,

        [Description("Token inválido.")]
        TokenInvalid = -31,

        [Description("Nenhum {0} enviado")]
        NoneField = -32,
    }
}
