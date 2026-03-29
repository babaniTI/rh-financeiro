using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using rh.financeiro.Domain.Common.Http.Response;
using rh.financeiro.Domain.Enums;
using rh.financeiro.Domain.Interfaces.Service;
using rh.financeiro.Domain.Interfaces.Service.Auth;

public class JWTAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authService = context.HttpContext.RequestServices.GetService(typeof(IAuthService)) as IAuthService;

        var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            context.Result = new ObjectResult(
                     ResponseApi.FormatResponse(
                         ReturnStatus.Unauthorized,
                         ReturnCodes.TokenInvalid,
                         false,
                         "Usuário não autenticado"
                     ));
            return;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();

        ClaimsPrincipal user = authService.GetPrincipal(token);

        if (user == null)
        {
            context.Result = new ObjectResult(
                    ResponseApi.FormatResponse(
                        ReturnStatus.Unauthorized,
                        ReturnCodes.TokenInvalid,
                        false,
                        "Usuário não autenticado"
                    ));
        }

        context.HttpContext.User = user;
    }
}
