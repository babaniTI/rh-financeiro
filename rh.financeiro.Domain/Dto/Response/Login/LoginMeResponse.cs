using rh.financeiro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Response.Login
{
    public class LoginMeResponse
    {
        public string id { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public string empresaId { get; set; }
        public Empresa empresa { get; set; }
        public bool Ativo { get; set;}
        public string createdAt { get; set; } // ISO 8601

        public string updatedAt { get; set; } // ISO 8601

    }
}
