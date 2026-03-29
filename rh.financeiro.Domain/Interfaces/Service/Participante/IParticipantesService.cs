using rh.financeiro.Domain.Dto.Paginacao;
using rh.financeiro.Domain.Dto.Request.Participantes.BuscarParticipantes;
using rh.financeiro.Domain.Dto.Request.Participantes.CriarParticipante;
using rh.financeiro.Domain.Dto.Request.Participantes.EditarParticipante;
using rh.financeiro.Domain.Dto.Response.Participantes.BuscarParticipantes;
using rh.financeiro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Interfaces.Service.Participantes
{
    public interface IParticipantesService
    {
        Task<PaginacaoResponse<BuscarParticipantesResponse>> BuscarParticipantes(BuscarParticipantesRequest request,string UsuarioId);
        Task<Participante?> BuscarParticipantePorId(string id, string UsuarioId);
        Task<Participante?> CriarParticipante(CriarParticipanteRequest request,string UsuarioId);
        Task<Participante?> EditarParticipantePorId(EditarParticipanteRequest request, string id, string UsuarioId);
    }
}
