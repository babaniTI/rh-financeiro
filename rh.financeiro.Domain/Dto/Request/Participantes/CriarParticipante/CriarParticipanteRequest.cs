using rh.financeiro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Request.Participantes.CriarParticipante
{
    public class CriarParticipanteRequest
    {
        public string TipoPessoa { get; set; }
        public string cpfcnpj { get; set; }
        public string razaoSocial { get; set; }
        public string tipo { get; set; }
    }

    
}
