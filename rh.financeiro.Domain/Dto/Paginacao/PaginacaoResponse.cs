using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Paginacao
{
    public class PaginacaoResponse<T>
    {
        public int total { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public int totalPages { get; set; }
        public List<T> data { get; set; }
    }
}
