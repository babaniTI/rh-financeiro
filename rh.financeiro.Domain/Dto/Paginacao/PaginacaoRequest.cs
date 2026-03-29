using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Paginacao
{
    public class PaginacaoRequest
    {
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 25;
    }
}
