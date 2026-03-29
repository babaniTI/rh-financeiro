using Microsoft.AspNetCore.Mvc;
using rh.financeiro.CrossCuting;
using rh.financeiro.Data.Context.Default;
using rh.financeiro.Domain.Common.Http.Response;
using rh.financeiro.Domain.Dto.Request.Login;
using rh.financeiro.Domain.Enums;
using rh.financeiro.Domain.Interfaces.Service.Auth;
using rh.financeiro.Domain.Interfaces.UnitOfWorks;

namespace rh.financeiro.webservice.Controllers.Auth
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ResponseApi>> Login([FromBody] LoginRequestDTO request)
        {
            #region Consistencias
            if (request is null)
            {
                return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                    $"Validations Login: {Utils.GetEnumDescription(ReturnCodes.EmptyRequest)}"));
            }
            #endregion

            #region Logica
            try
            {
                var response = await _authService.Login(request);

                if (response == null)
                {
                    return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.Unauthorized, ReturnCodes.AccessDenied, false,
                        "Credenciais Inválidas", null));
                }

                return StatusCode((int)ReturnStatus.Ok, ResponseApi.FormatResponse(ReturnStatus.Ok, ReturnCodes.Ok, true,
                    "AccessToken Obtido com Sucesso", response));
            }
            catch (Exception ex)
            {
                return new ResponseApi()
                {
                    Code = (int)ReturnCodes.ExceptionEx,
                    Data = DateTime.Now,
                    Message = $"MESSAGE => {ex.Message} || INNER EXCEPTION => {ex.InnerException}",
                    Status = (int)ReturnStatus.InternalServerError,
                    Success = false
                };
            }
            #endregion
        }

        [HttpPost("me")]
        public async Task<ActionResult<ResponseApi>> LoginMe()
        {
            #region Consistencias
            #endregion

            #region Logica
            try
            {
                string? UsuarioId = Utils.GetUserId(HttpContext);
                var response = await _authService.LoginMe(UsuarioId);

                if (response == null)
                {
                    return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.Unauthorized, ReturnCodes.AccessDenied, false,
                        "Usuario inválido e/ou Usuario sem empresa", null));
                }

                return StatusCode((int)ReturnStatus.Ok, ResponseApi.FormatResponse(ReturnStatus.Ok, ReturnCodes.Ok, true,
                    "Contexto do Usuario obtido com sucesso!", response));
            }
            catch (Exception ex)
            {
                return new ResponseApi()
                {
                    Code = (int)ReturnCodes.ExceptionEx,
                    Data = DateTime.Now,
                    Message = $"MESSAGE => {ex.Message} || INNER EXCEPTION => {ex.InnerException}",
                    Status = (int)ReturnStatus.InternalServerError,
                    Success = false
                };
            }
            #endregion
        }

        [HttpPost("Logout")]
        public async Task<ActionResult<ResponseApi>> Logout()
        {
            #region Consistencias
            #endregion

            #region Logica
            try
            {
                var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();

                if (string.IsNullOrEmpty(authHeader))
                    return Unauthorized();

                var token = authHeader.StartsWith("Bearer ")
                    ? authHeader.Substring("Bearer ".Length).Trim()
                    : authHeader;

                return StatusCode((int)ReturnStatus.Ok, ResponseApi.FormatResponse(ReturnStatus.Ok, ReturnCodes.Ok, true));
            }
            catch (Exception ex)
            {
                return new ResponseApi()
                {
                    Code = (int)ReturnCodes.ExceptionEx,
                    Data = DateTime.Now,
                    Message = $"MESSAGE => {ex.Message} || INNER EXCEPTION => {ex.InnerException}",
                    Status = (int)ReturnStatus.InternalServerError,
                    Success = false
                };
            }
            #endregion
        }

        #region LOG Method
        #endregion
    }
}
