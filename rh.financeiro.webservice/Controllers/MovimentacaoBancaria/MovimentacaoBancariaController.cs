using Microsoft.AspNetCore.Mvc;
using rh.financeiro.CrossCuting;
using rh.financeiro.Domain.Common.Http.Response;
using rh.financeiro.Domain.Dto.Request.MovimentacaoBancaria.BuscarMovimentacoesBancarias;
using rh.financeiro.Domain.Enums;
using rh.financeiro.Domain.Interfaces.Service.MovimentacaoBancaria;

namespace rh.financeiro.webservice.Controllers.MovimentacaoBancaria
{
    [ApiController]
    [Route("[controller]")]
    public class MovimentacaoBancariaController : ControllerBase
    {
        private readonly IMovimentacaoBancariaService _movimentacaoBancariaService;

        public MovimentacaoBancariaController(IMovimentacaoBancariaService movimentacaoBancariaService)
        {
            _movimentacaoBancariaService = movimentacaoBancariaService;
        }

        [HttpGet()]
        public async Task<ActionResult<ResponseApi>> BuscarMovimentacoesBancarias([FromQuery] BuscarMovimentacoesBancariasRequest request)
        {
            #region Consistencias
            if (request is null)
            {
                return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                    $"Validations BuscarMovimentacoesBancariasRequest: {Utils.GetEnumDescription(ReturnCodes.EmptyRequest)}"));
            }
            #endregion

            #region Logica
            try
            {
                string UsuarioId = Utils.GetUserId(HttpContext);
                var response = await _movimentacaoBancariaService.BuscarMovimentacoesBancarias(request, UsuarioId);

                if (response.total == 0)
                {
                    return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                        "Movimentacoes Bancarias não encontrados", null));
                }

                return StatusCode((int)ReturnStatus.Ok, ResponseApi.FormatResponse(ReturnStatus.Ok, ReturnCodes.Ok, true,
                    "Movimentacoes Bancarias encontrados com sucsso", response));
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

        [HttpPost("ofx/importar")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ResponseApi>> ImportarMovimentacaoBancariaOfx(IFormFile file)
        {
            #region Consistencias
            if (file == null || file.Length == 0)
            {
                return StatusCode((int)ReturnStatus.BadRequest,
                    ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                    "Arquivo OFX não informado"));
            }

            var extensao = Path.GetExtension(file.FileName).ToLower();
            if (extensao != ".ofx")
            {
                return StatusCode((int)ReturnStatus.BadRequest,
                    ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.InvalidValue, false,
                    "Arquivo deve ser do tipo OFX"));
            }
            #endregion

            #region Logica
            try
            {
                string UsuarioId = Utils.GetUserId(HttpContext);
                string xmlContent;
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    xmlContent = await reader.ReadToEndAsync();
                }

                bool success = await _movimentacaoBancariaService.ImportarOfx(UsuarioId, xmlContent);

                if (!success)
                {
                    return StatusCode((int)ReturnStatus.BadRequest,
                        ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                        "OFX invalido e/ou Conta financeira não encontrada"));
                }

                return StatusCode((int)ReturnStatus.Created,
                    ResponseApi.FormatResponse(ReturnStatus.Created, ReturnCodes.Ok, true,
                    "Movimentacao bancaria importado com sucesso"));
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
