using Microsoft.AspNetCore.Mvc;
using rh.financeiro.CrossCuting;
using rh.financeiro.Domain.Common.Http.Response;
using rh.financeiro.Domain.Dto.Request.ContasFinanceiras.BuscarContasFinanceiras;
using rh.financeiro.Domain.Dto.Request.Participantes.BuscarParticipantes;
using rh.financeiro.Domain.Enums;
using rh.financeiro.Domain.Interfaces.Service.ContaFinanceiras;
using rh.financeiro.Domain.Interfaces.Service.Participantes;
using rh.financeiro.Services.Services.Participantes;

namespace rh.financeiro.webservice.Controllers.ContaFinanceira
{
    [ApiController]
    [Route("[controller]")]
    public class ContaFinanceiraController : ControllerBase
    {
        private readonly IContasFinanceirasService _contaFinanceiraService;

        public ContaFinanceiraController(IContasFinanceirasService contaFinanceiraService)
        {
            _contaFinanceiraService = contaFinanceiraService;
        }

        [HttpGet()]
        public async Task<ActionResult<ResponseApi>> BuscarContasFinaceiras([FromQuery] BuscarContasFinanceirasRequest request)
        {
            #region Consistencias
            if (request is null)
            {
                return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                    $"Validations BuscarContasFinaceiras: {Utils.GetEnumDescription(ReturnCodes.EmptyRequest)}"));
            }
            #endregion

            #region Logica
            try
            {
                string UsuarioId = Utils.GetUserId(HttpContext);
                var response = await _contaFinanceiraService.BuscarContasFinanceiras(request, UsuarioId);

                if (response.total == 0)
                {
                    return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                        "Contas Financeiras não encontrados", null));
                }

                return StatusCode((int)ReturnStatus.Ok, ResponseApi.FormatResponse(ReturnStatus.Ok, ReturnCodes.Ok, true,
                    "Contas Financeiras encontrados com sucsso", response));
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
        public async Task<ActionResult<ResponseApi>> BuscarContaFinanceiraPorId([FromRoute] string id)
        {
            #region Consistencias
            if (string.IsNullOrEmpty(id))
            {
                return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                    $"Validations BuscarContaFinanceiraPorId: {Utils.GetEnumDescription(ReturnCodes.EmptyRequest)}"));
            }
            #endregion

            #region Logica
            try
            {
                string UsuarioId = Utils.GetUserId(HttpContext);
                var response = await _contaFinanceiraService.BuscarContaFinanceiraPorId(id, UsuarioId);

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
