using rh.financeiro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Response.Login
{
    public class LoginResponseDTO
    {
        public string accessToken { get; set; }
        public int expiresIn { get; set; }
        public UsuarioInfos usuario { get; set; }
    }

    public class UsuarioInfos
    {
        public string id { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public string empresaId { get; set; }
        public EmpresaInfos Empresa { get; set; }
    }

    public class EmpresaInfos
    {
        public string id { get; set; }
        public string razaoSocial { get; set; }
        public string cnpj { get; set; }
    }
}
