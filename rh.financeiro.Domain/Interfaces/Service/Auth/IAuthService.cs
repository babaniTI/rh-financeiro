using rh.financeiro.Domain.Dto.Request.Login;
using rh.financeiro.Domain.Dto.Response.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Interfaces.Service.Auth
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> Login(LoginRequestDTO request);
        Task<LoginMeResponse?> LoginMe(string UsuarioId);
        ClaimsPrincipal GetPrincipal(string token);
    }
}
