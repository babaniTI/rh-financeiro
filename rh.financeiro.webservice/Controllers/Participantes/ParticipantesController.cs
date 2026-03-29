using Microsoft.AspNetCore.Mvc;
using rh.financeiro.CrossCuting;
using rh.financeiro.Domain.Common.Http.Response;
using rh.financeiro.Domain.Dto.Request.Login;
using rh.financeiro.Domain.Dto.Request.Participantes.BuscarParticipantes;
using rh.financeiro.Domain.Dto.Request.Participantes.CriarParticipante;
using rh.financeiro.Domain.Dto.Request.Participantes.EditarParticipante;
using rh.financeiro.Domain.Enums;
using rh.financeiro.Domain.Interfaces.Service.Auth;
using rh.financeiro.Domain.Interfaces.Service.Participantes;
using rh.financeiro.Services.Services.Auth;

namespace rh.financeiro.webservice.Controllers.Participantes
{
    [ApiController]
    [Route("[controller]")]
    public class ParticipantesController : ControllerBase
    {
        private readonly IParticipantesService _participantesService;

        public ParticipantesController(IParticipantesService participantesService)
        {
            _participantesService = participantesService;
        }

        [HttpPost()]
        public async Task<ActionResult<ResponseApi>> CriarParticipante([FromBody] CriarParticipanteRequest request)
        {
            #region Consistencias
            if (request is null)
            {
                return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                    $"Validations CriarParticipante: {Utils.GetEnumDescription(ReturnCodes.EmptyRequest)}"));
            }
            #endregion

            #region Logica
            try
            {
                string UsuarioId = Utils.GetUserId(HttpContext);
                var response = await _participantesService.CriarParticipante(request,UsuarioId);

                if (response == null)
                {
                    return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyField, false,
                        "Participante já existe!"));
                }

                return StatusCode((int)ReturnStatus.Created, ResponseApi.FormatResponse(ReturnStatus.Created, ReturnCodes.Ok, true,
                    "Participante criado com sucesso", response));
            }
            catch (Exception ex)
            {
                return new ResponseApi()
                {
                    Code = (int)ReturnCodes.ExceptionEx,
                    Data = DateTime.Now,
                    Message = $"MESSAGE => {ex.Message} || INNER EXCEPTION => {ex.InnerException}",
                    Status = (int)ReturnStatus.InternalServerError,
                    Success = false
                };
            }
            #endregion
        }

        [HttpPut()]
        public async Task<ActionResult<ResponseApi>> EditarParticipante([FromBody] EditarParticipanteRequest request, [FromRoute] string id)
        {
            #region Consistencias
            if (request is null)
            {
                return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                    $"Validations EditarParticipante: {Utils.GetEnumDescription(ReturnCodes.EmptyRequest)}"));
            }
            #endregion

            #region Logica
            try
            {
                string UsuarioId = Utils.GetUserId(HttpContext);
                var response = await _participantesService.EditarParticipantePorId(request,id, UsuarioId);

                if (response == null)
                {
                    return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyField, false,
                        "Participante não existe!"));
                }

                return StatusCode((int)ReturnStatus.Ok, ResponseApi.FormatResponse(ReturnStatus.Ok, ReturnCodes.Ok, true,
                    "Participante atualizado com sucesso", response));
            }
            catch (Exception ex)
            {
                return new ResponseApi()
                {
                    Code = (int)ReturnCodes.ExceptionEx,
                    Data = DateTime.Now,
                    Message = $"MESSAGE => {ex.Message} || INNER EXCEPTION => {ex.InnerException}",
                    Status = (int)ReturnStatus.InternalServerError,
                    Success = false
                };
            }
            #endregion
        }



        [HttpGet()]
        public async Task<ActionResult<ResponseApi>> BuscarParticipantes([FromQuery] BuscarParticipantesRequest request)
        {
            #region Consistencias
            if (request is null)
            {
                return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                    $"Validations BuscarParticipantes: {Utils.GetEnumDescription(ReturnCodes.EmptyRequest)}"));
            }
            #endregion

            #region Logica
            try
            {
                string UsuarioId = Utils.GetUserId(HttpContext);
                var response = await _participantesService.BuscarParticipantes(request,UsuarioId);

                if (response.total == 0)
                {
                    return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                        "Partipantes não encontrado", null));
                }

                return StatusCode((int)ReturnStatus.Ok, ResponseApi.FormatResponse(ReturnStatus.Ok, ReturnCodes.Ok, true,
                    "Participantes encontrados com sucsso", response));
            }
            catch (Exception ex)
            {
                return new ResponseApi()
                {
                    Code = (int)ReturnCodes.ExceptionEx,
                    Data = DateTime.Now,
                    Message = $"MESSAGE => {ex.Message} || INNER EXCEPTION => {ex.InnerException}",
                    Status = (int)ReturnStatus.InternalServerError,
                    Success = false
                };
            }
            #endregion
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseApi>> BuscarParticipantePorId([FromRoute] string id)
        {
            #region Consistencias
            if (string.IsNullOrEmpty(id))
            {
                return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                    $"Validations BuscarParticipantePorId: {Utils.GetEnumDescription(ReturnCodes.EmptyRequest)}"));
            }
            #endregion

            #region Logica
            try
            {
                string UsuarioId = Utils.GetUserId(HttpContext);
                var response = await _participantesService.BuscarParticipantePorId(id,UsuarioId);

                if (response == null)
                {
                    return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                        "Partipante não encontrado", null));
                }

                return StatusCode((int)ReturnStatus.Ok, ResponseApi.FormatResponse(ReturnStatus.Ok, ReturnCodes.Ok, true,
                    "Participante encontrado com sucsso", response));
            }
            catch (Exception ex)
            {
                return new ResponseApi()
                {
                    Code = (int)ReturnCodes.ExceptionEx,
                    Data = DateTime.Now,
                    Message = $"MESSAGE => {ex.Message} || INNER EXCEPTION => {ex.InnerException}",
                    Status = (int)ReturnStatus.InternalServerError,
                    Success = false
                };
            }
            #endregion
        }

    }
}
