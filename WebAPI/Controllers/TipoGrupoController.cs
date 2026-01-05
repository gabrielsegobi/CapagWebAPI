using Application.Filters;
using Application.Queries.TipoGrupos;
using Domain.Contracts.TipoGrupos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TipoGrupoController : BaseApiController
    {
        public TipoGrupoController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAllTiposGrupos([FromQuery] TipoGrupoFilter filter)
        {
            var response = await mediator.Send(new GetAllTiposGruposQuery(filter));
            return Ok(response);
        }

        [HttpGet("variaveis/{id}")]
        public async Task<IActionResult> GetVariaveis(long id, [FromQuery] GetVariaveisRequest request)
        {
            var response = await mediator.Send(new GetVariaveisQuery(id, request));
            return Ok(response);
        }

    }
}
