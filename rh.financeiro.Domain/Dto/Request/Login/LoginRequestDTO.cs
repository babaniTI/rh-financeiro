using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Request.Login
{
    public class LoginRequestDTO
    {
        public string email { get; set; }
        public string senha { get; set; }
    }
}
