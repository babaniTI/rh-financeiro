using rh.financeiro.Domain.Dto.Paginacao;
using rh.financeiro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Request.Participantes.BuscarParticipantes
{
    public class BuscarParticipantesRequest : PaginacaoRequest
    {
        public string? tipo { get; set; }
        public bool? ativo { get; set; } = true;
        public string? search { get; set; }
    }
}
