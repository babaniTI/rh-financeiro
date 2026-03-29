using rh.financeiro.Domain.Dto.Paginacao;
using rh.financeiro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Response.Participantes.BuscarParticipantes
{
    public class BuscarParticipantesResponse
    {
        public string id { get; set; }
        public string codigo { get; set; } 
        public string nome { get; set; }
        public string cpfcnpj { get; set; }
        public TipoParticipante? tipo { get; set; }
        public bool Ativo { get; set; }
        public DateTime created_At { get; set; }
        public DateTime updated_At { get; set; }

    }
}
