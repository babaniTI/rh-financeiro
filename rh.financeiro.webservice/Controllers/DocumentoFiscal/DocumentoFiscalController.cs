using Microsoft.AspNetCore.Mvc;
using rh.financeiro.CrossCuting;
using rh.financeiro.Domain.Common.Http.Response;
using rh.financeiro.Domain.Dto.Request.ContasFinanceiras.BuscarContasFinanceiras;
using rh.financeiro.Domain.Dto.Request.DocumentoFiscal.BuscarDocumentosFiscais;
using rh.financeiro.Domain.Enums;
using rh.financeiro.Domain.Interfaces.Service.ContaFinanceiras;
using rh.financeiro.Domain.Interfaces.Service.DocumentoFiscal;

namespace rh.financeiro.webservice.Controllers.DocumentoFiscal
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentoFiscalController : ControllerBase
    {
        private readonly IDocumentoFiscalService _documentoFiscalService;

        public DocumentoFiscalController(IDocumentoFiscalService documentoFiscalService)
        {
            _documentoFiscalService = documentoFiscalService;
        }

        [HttpGet()]
        public async Task<ActionResult<ResponseApi>> BuscarDocumentosFiscais([FromQuery] BuscarDocumentosFiscaisRequest request)
        {
            #region Consistencias
            if (request is null)
            {
                return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                    $"Validations BuscarDocumentosFiscais: {Utils.GetEnumDescription(ReturnCodes.EmptyRequest)}"));
            }
            #endregion

            #region Logica
            try
            {
                string UsuarioId = Utils.GetUserId(HttpContext);
                var response = await _documentoFiscalService.BuscarDocumentosFiscais(request, UsuarioId);

                if (response.total == 0)
                {
                    return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                        "Documentos Fiscais não encontrados", null));
                }

                return StatusCode((int)ReturnStatus.Ok, ResponseApi.FormatResponse(ReturnStatus.Ok, ReturnCodes.Ok, true,
                    "Documentos Fiscais encontrados com sucsso", response));
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

        [HttpPost("xml/importar")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ResponseApi>> ImportarDocumentoFiscalXml(IFormFile file)
        {
            #region Consistencias
            if (file == null || file.Length == 0)
            {
                return StatusCode((int)ReturnStatus.BadRequest,
                    ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                    "Arquivo XML não informado"));
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

                bool success = await _documentoFiscalService.ImportarDocumentoFiscalXml(UsuarioId, xmlContent);

                if (!success)
                {
                    return StatusCode((int)ReturnStatus.BadRequest,
                        ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                        "Xml invalido"));
                }

                return StatusCode((int)ReturnStatus.Created,
                    ResponseApi.FormatResponse(ReturnStatus.Created, ReturnCodes.Ok, true,
                    "Xml da Nota importado com sucesso"));
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
