using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using rh.financeiro.CrossCuting;
using rh.financeiro.Domain.Common.Http.Response;
using rh.financeiro.Domain.Dto.Request.CategoriaFinanceira.CriarCategoriaFinanceira;
using rh.financeiro.Domain.Dto.Request.Jobs.ImportarDocumentosAndGerarTitulos;
using rh.financeiro.Domain.Enums;
using rh.financeiro.Domain.Interfaces.Service.CategoriaFinanceiras;
using rh.financeiro.Domain.Interfaces.Service.Jobs;
using rh.financeiro.Services.Services.CategoriaFinanceiras;

namespace rh.financeiro.webservice.Controllers.Jobs
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IJobsService _jobsService;

        public JobsController(IJobsService jobsService)
        {
            _jobsService = jobsService;
        }

        [HttpPost("documentos/importar")]
        public async Task<ActionResult<ResponseApi>> ImportarDocumentosAndGerarTitulos([FromQuery] ImportarDocumentosAndGerarTitulosRequest request)
        {
            #region Consistencias
            if (request is null)
            {
                return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                    $"Validations ImportarDocumentosAndGerarTitulos: {Utils.GetEnumDescription(ReturnCodes.EmptyRequest)}"));
            }
            #endregion

            #region Logica
            try
            {
                (bool success, string Message) = await _jobsService.ImportarDocumentosAndGerarTitulos(request);

                if (!success)
                {
                    return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyField, false,
                        Message));
                }

                return StatusCode((int)ReturnStatus.Created, ResponseApi.FormatResponse(ReturnStatus.Created, ReturnCodes.Ok, true,
                    Message));
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

        [HttpPost("documentos/atualizacao")]
        public async Task<ActionResult<ResponseApi>> AtualizarDocumentosAndTitulos([FromQuery] string Cnpj)
        {
            #region Consistencias
            #endregion

            #region Logica
            try
            {
                (bool success, string Message) = await _jobsService.AtualizarDocumentosAndTitulos(Cnpj);

                if (!success)
                {
                    return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyField, false,
                        Message));
                }

                return StatusCode((int)ReturnStatus.Created, ResponseApi.FormatResponse(ReturnStatus.Created, ReturnCodes.Ok, true,
                    Message));
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
