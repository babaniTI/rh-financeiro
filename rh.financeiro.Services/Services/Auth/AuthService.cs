using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using rh.financeiro.CrossCuting;
using rh.financeiro.Data.Context.Default;
using rh.financeiro.Domain.Dto.Request.Login;
using rh.financeiro.Domain.Dto.Response.Login;
using rh.financeiro.Domain.Entities;
using rh.financeiro.Domain.Interfaces.Service.Auth;
using rh.financeiro.Domain.Interfaces.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Services.Services.Auth
{
    public class AuthService : IAuthService
    {
        #region Fields
        private readonly IUnitOfWork<DefaultContext> _unitOfWork;
        private readonly IConfiguration _configuration;
        #endregion

        #region CONS
        public AuthService(IConfiguration configuration,IUnitOfWork<DefaultContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }
        #endregion

        #region Public Methods
        public async Task<LoginResponseDTO> Login(LoginRequestDTO request)
        {
            #region Fields
            string? email = request.email;
            string? senha = request.senha;
            #endregion

            #region Logic
            try
            {
                bool isCredencialValida = await ValidarCredenciaisAsync(email, senha);
                if (!isCredencialValida) return null;

                var UsuarioInfos = await ObterUsuarioInfos(email, senha);
                var AccessToken = await GerarTokenJWT(email);

                return new LoginResponseDTO()
                {
                    accessToken = AccessToken,
                    expiresIn = Convert.ToInt32((TimeSpan.FromDays(1)).TotalSeconds),
                    usuario = UsuarioInfos
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion
        }

        public async Task<LoginMeResponse?> LoginMe(string UsuarioId)
        {
            #region Fields
            var repoUsuarios = _unitOfWork.Repository<Usuario>().QueryableObject();
            var repoUsuarioEmpresa = _unitOfWork.Repository<UsuarioEmpresa>().QueryableObject();
            var repoEmpresa = _unitOfWork.Repository<Empresa>().QueryableObject();
            #endregion

            #region Logic
            try
            {
                var UsuarioXEmpresa = await (
                    from usu in repoUsuarios 
                    join usue in repoUsuarioEmpresa on usu.Id equals usue.UsuarioId
                    join e in  repoEmpresa on usue.EmpresaId equals e.Id
                    where usu.Id.ToString() == UsuarioId
                    select new
                    {
                        Usuario = usu,
                        Empresa = e
                    }
                ).FirstOrDefaultAsync();

                if (UsuarioXEmpresa == null)
                    return null;

                // Separando as entidades de Usuario e Empresa
                var Usuario = UsuarioXEmpresa.Usuario;
                var Empresa = UsuarioXEmpresa.Empresa;

                return new LoginMeResponse()
                {
                    id = Usuario.Id.ToString(),
                    nome = Usuario.Nome,
                    email = Usuario.Email,
                    empresaId = Empresa.Id.ToString(),
                    empresa = Empresa,
                    Ativo = Usuario.Ativo,
                    createdAt = Usuario.CreatedAt.ToString("O"),
                    updatedAt = Usuario.CreatedAt.ToString("O")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion
        }
        public ClaimsPrincipal GetPrincipal(string token)
        {
            #region Fields
            string? Signature = _configuration["JWT:SecretKey"];
            #endregion

            #region Logic
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);

                if (jwtToken == null)
                    return null;

                byte[] key = Convert.FromBase64String(Signature);

                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                      parameters, out SecurityToken securityToken);

                return principal;
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
        private async Task<bool> ValidarCredenciaisAsync(string email, string senha)
        {
            #region Fields
            var repoUsuarios = _unitOfWork.Repository<Usuario>().QueryableObject();
            var repoUsuarioEmpresa = _unitOfWork.Repository<UsuarioEmpresa>().QueryableObject();
            #endregion

            try
            {
                var senhaCriptografada = Utils.EncryptCIS(senha);
                var existe = await (
                   from usu in repoUsuarios
                   join usue in repoUsuarioEmpresa
                       on usu.Id equals usue.UsuarioId
                   where usu.Email == email
                         && usu.SenhaHash == senhaCriptografada
                   select usu.Id
               ).AnyAsync();

                return existe;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private async Task<UsuarioInfos?> ObterUsuarioInfos(string email, string senha)
        {
            #region Fields
            var repoUsuarios = _unitOfWork.Repository<Usuario>().QueryableObject();
            var repoUsuarioEmpresa = _unitOfWork.Repository<UsuarioEmpresa>().QueryableObject();
            var repoEmpresa = _unitOfWork.Repository<Empresa>().QueryableObject();
            #endregion

            try
            {
                var senhaCriptografada = Utils.EncryptCIS(senha);
                var usuario = await (
                   from usu in repoUsuarios
                   join usue in repoUsuarioEmpresa on usu.Id equals usue.UsuarioId
                   join e in repoEmpresa on usue.EmpresaId equals e.Id
                   where usu.Email == email
                         && usu.SenhaHash == senhaCriptografada
                   select new UsuarioInfos
                   {
                       id = usu.Id.ToString(),
                       nome = usu.Nome,
                       email = usu.Email,
                       empresaId = usue.EmpresaId.ToString(),
                       Empresa = new()
                       {
                           id = e.Id.ToString(),
                           razaoSocial = e.Nome,
                           cnpj = e.Cnpj.ToString(),
                       }
                   }
               ).FirstOrDefaultAsync();

                return usuario;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private async Task<string> GerarTokenJWT(string email)
        {
            try
            {
                var UsuarioID = await _unitOfWork.Repository<Usuario>()
                   .QueryableObject()
                   .Where(x => x.Email == email)
                   .Select(x => x.Id)
                   .FirstOrDefaultAsync();

                string? Signature = _configuration["JWT:SecretKey"];
                byte[] key = Convert.FromBase64String(Signature);
                SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);

                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.CreateToken(new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, UsuarioID.ToString()) }),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = new SigningCredentials(securityKey,
                    SecurityAlgorithms.HmacSha256Signature)
                });

                return handler.WriteToken(securityToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
    #endregion
}

