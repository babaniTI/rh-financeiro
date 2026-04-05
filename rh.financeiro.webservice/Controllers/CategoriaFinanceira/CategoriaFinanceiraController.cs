using Microsoft.AspNetCore.Mvc;
using rh.financeiro.CrossCuting;
using rh.financeiro.Domain.Common.Http.Response;
using rh.financeiro.Domain.Dto.Request.CategoriaFinanceira.BuscarCategoriasFinanceiras;
using rh.financeiro.Domain.Dto.Request.CategoriaFinanceira.CriarCategoriaFinanceira;
using rh.financeiro.Domain.Dto.Request.CategoriaFinanceira.EditarCategoriaFinanceira;
using rh.financeiro.Domain.Dto.Request.ContasFinanceiras.BuscarContasFinanceiras;
using rh.financeiro.Domain.Dto.Request.ContasFinanceiras.CriarContaFinanceira;
using rh.financeiro.Domain.Dto.Request.ContasFinanceiras.EditarContaFinanceira;
using rh.financeiro.Domain.Enums;
using rh.financeiro.Domain.Interfaces.Service.CategoriaFinanceiras;
using rh.financeiro.Domain.Interfaces.Service.ContaFinanceiras;

namespace rh.financeiro.webservice.Controllers.CategoriaFinanceira
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriaFinanceiraController : ControllerBase
    {
        private readonly ICategoriaFinanceiraService _categoriaFinanceiraService;

        public CategoriaFinanceiraController(ICategoriaFinanceiraService categoriaFinanceiraService)
        {
            _categoriaFinanceiraService = categoriaFinanceiraService;
        }

        [HttpPost()]
        public async Task<ActionResult<ResponseApi>> CriarCategoriaFinanceira([FromBody] CriarCategoriaFinanceiraRequest request)
        {
            #region Consistencias
            if (request is null)
            {
                return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                    $"Validations CriarCategoriaFinanceira: {Utils.GetEnumDescription(ReturnCodes.EmptyRequest)}"));
            }
            #endregion

            #region Logica
            try
            {
                string UsuarioId = Utils.GetUserId(HttpContext);
                var response = await _categoriaFinanceiraService.CriarCategoriaFinanceira(request, UsuarioId);

                if (response == null)
                {
                    return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyField, false,
                        "Categoria Financeira já existe!"));
                }

                return StatusCode((int)ReturnStatus.Created, ResponseApi.FormatResponse(ReturnStatus.Created, ReturnCodes.Ok, true,
                    "Categoria Financeira criado com sucesso", response));
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

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseApi>> EditarCategoriaFinanceira([FromBody] EditarCategoriaFinanceiraRequest request, [FromRoute] string id)
        {
            #region Consistencias
            if (request is null)
            {
                return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                    $"Validations EditarCategoriaFinanceira: {Utils.GetEnumDescription(ReturnCodes.EmptyRequest)}"));
            }
            #endregion

            #region Logica
            try
            {
                string UsuarioId = Utils.GetUserId(HttpContext);
                var response = await _categoriaFinanceiraService.EditarCategoriaFinanceiraPorId(request, id, UsuarioId);

                if (response == null)
                {
                    return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyField, false,
                        "Categoria Financeira não existe!"));
                }

                return StatusCode((int)ReturnStatus.Ok, ResponseApi.FormatResponse(ReturnStatus.Ok, ReturnCodes.Ok, true,
                    "Categoria Financeira atualizado com sucesso", response));
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
        public async Task<ActionResult<ResponseApi>> BuscarCategoriasFinaceiras([FromQuery] BuscarCategoriasFinanceirasRequest request)
        {
            #region Consistencias
            if (request is null)
            {
                return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                    $"Validations BuscarCategoriasFinaceiras: {Utils.GetEnumDescription(ReturnCodes.EmptyRequest)}"));
            }
            #endregion

            #region Logica
            try
            {
                string UsuarioId = Utils.GetUserId(HttpContext);
                var response = await _categoriaFinanceiraService.BuscarCategoriasFinanceiras(request, UsuarioId);

                if (response.total == 0)
                {
                    return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                        "Categorias Financeiras não encontrados", null));
                }

                return StatusCode((int)ReturnStatus.Ok, ResponseApi.FormatResponse(ReturnStatus.Ok, ReturnCodes.Ok, true,
                    "Categorias Financeiras encontrados com sucsso", response));
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
        public async Task<ActionResult<ResponseApi>> BuscarCategoriaFinanceiraPorId([FromRoute] string id)
        {
            #region Consistencias
            if (string.IsNullOrEmpty(id))
            {
                return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                    $"Validations BuscarCategoriaFinanceiraPorId: {Utils.GetEnumDescription(ReturnCodes.EmptyRequest)}"));
            }
            #endregion

            #region Logica
            try
            {
                string UsuarioId = Utils.GetUserId(HttpContext);
                var response = await _categoriaFinanceiraService.BuscarCategoriaFinanceiraPorId(id, UsuarioId);

                if (response == null)
                {
                    return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyRequest, false,
                        "Categoria não encontrado", null));
                }

                return StatusCode((int)ReturnStatus.Ok, ResponseApi.FormatResponse(ReturnStatus.Ok, ReturnCodes.Ok, true,
                    "Categoria encontrado com sucsso", response));
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

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseApi>> DesativarOuReativarCategoriaFinaceira([FromRoute] string id, [FromQuery] string Acao)
        {
            #region Consistencias
            #endregion

            #region Logica
            try
            {
                string UsuarioId = Utils.GetUserId(HttpContext);
                var success = await _categoriaFinanceiraService.DesativarOuReativarCategoriaFinaceiraPorId(id, Acao, UsuarioId);

                if (!success)
                {
                    return StatusCode((int)ReturnStatus.BadRequest, ResponseApi.FormatResponse(ReturnStatus.BadRequest, ReturnCodes.EmptyField, false,
                        "Categoria Financeira não existe!"));
                }

                return StatusCode((int)ReturnStatus.Ok, ResponseApi.FormatResponse(ReturnStatus.Ok, ReturnCodes.Ok, true,
                    "Categoria Financeira atualizado com sucesso"));
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
