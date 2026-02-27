using Application.Filters;
using Application.Queries.Indicadores;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class IndicadoresController : BaseApiController

    {
        public IndicadoresController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAllIndicadores([FromQuery] IndicadorFilter filter)
        {
            var response = await mediator.Send(new GetAllIndicadoresQuery(filter));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIndicadorById(long id)
        {
            var response = await mediator.Send(new GetIndicadorByIdQuery { Id = id });
            return Ok(response);
        }

        [HttpGet("calculos_indicaroes/{idEmpresa}")]
        public async Task<IActionResult> GetIndicadoresByEmpresaId(long idEmpresa, [FromQuery] CalcIndicadoresFilter filter)
        {
            var response = await mediator.Send(new GetAllCalcIndicaoresQuery(idEmpresa, filter));

            return Ok(response);
        }
    }
}