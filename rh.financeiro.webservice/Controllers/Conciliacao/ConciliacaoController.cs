using Microsoft.AspNetCore.Mvc;
using rh.financeiro.CrossCuting;
using rh.financeiro.Domain.Common.Http.Response;
using rh.financeiro.Domain.Dto.Request.MovimentacaoBancaria.BuscarMovimentacoesBancarias;
using rh.financeiro.Domain.Enums;
using rh.financeiro.Domain.Interfaces.Service.Conciliacao;
using rh.financeiro.Domain.Interfaces.Service.MovimentacaoBancaria;

namespace rh.financeiro.webservice.Controllers.Conciliacao
{
    [ApiController]
    [Route("[controller]")]
    public class ConciliacaoController : ControllerBase
    {
        private readonly IConciliacaoService _conciliacaoService;

        public ConciliacaoController(IConciliacaoService conciliacaoService)
        {
            _conciliacaoService = conciliacaoService;
        }

        [HttpGet("pendentes")]
        public async Task<ActionResult<ResponseApi>> BuscarConciliacoesPendentes()
        {
            #region Consistencias
            #endregion

            #region Logica
            try
            {
                var response = await _conciliacaoService.BuscarConciliacoesPendentes();

                return StatusCode((int)ReturnStatus.Ok, ResponseApi.FormatResponse(ReturnStatus.Ok, ReturnCodes.Ok, true,
                    "Conciliacoes pendentes  encontrados com sucesso", response));
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
