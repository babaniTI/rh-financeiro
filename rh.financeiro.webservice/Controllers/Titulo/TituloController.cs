using Microsoft.AspNetCore.Mvc;
using rh.financeiro.CrossCuting;
using rh.financeiro.Domain.Common.Http.Response;
using rh.financeiro.Domain.Dto.Request.DocumentoFiscal.BuscarDocumentosFiscais;
using rh.financeiro.Domain.Dto.Request.Titulos.BuscarTitulos;
using rh.financeiro.Domain.Enums;
using rh.financeiro.Domain.Interfaces.Service.DocumentoFiscal;
using rh.financeiro.Domain.Interfaces.Service.Titulo;
using rh.financeiro.Services.Services.DocumentoFiscais;

namespace rh.financeiro.webservice.Controllers.Titulo
{
    [ApiController]
    [Route("[controller]")]
    public class TituloController : ControllerBase
    {
        private readonly ITituloService _tituloService ;

        public TituloController(ITituloService tituloService)
        {
            _tituloService = tituloService;
        }

        [HttpGet()]
        public async Task<ActionResult<ResponseApi>> BuscarTitulos([FromQuery] BuscarTitulosRequest request)
        {
            #region Consistencias
            if (request is null)
            {
                return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                    $"Validations BuscarTitulos: {Utils.GetEnumDescription(ReturnCodes.EmptyRequest)}"));
            }
            #endregion

            #region Logica
            try
            {
                string UsuarioId = Utils.GetUserId(HttpContext);
                var response = await _tituloService.BuscarTitulos(request, UsuarioId);

                if (response.total == 0)
                {
                    return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                        "Titulos não encontrados", null));
                }

                return StatusCode((int)ReturnStatus.Ok, ResponseApi.FormatResponse(ReturnStatus.Ok, ReturnCodes.Ok, true,
                    "Titulos encontrados com sucesso", response));
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
