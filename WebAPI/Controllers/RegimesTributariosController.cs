using Application.Commands.RegimesTributarios;
using Application.Filters;
using Application.Queries.RegimesTributarios;
using Domain.Contracts.RegimesTributarios;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RegimesTributariosController : BaseApiController
    {
        public RegimesTributariosController(IMediator mediator) : base(mediator) { }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRegimeTributarioById(long id)
        {
            var response = await mediator.Send(new GetRegimeTributarioByIdQuery { Id = id });
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRegimesTributarios([FromQuery] RegimeTributarioFilter filter)
        {
            var response = await mediator.Send(new GetAllRegimesTributariosQuery(filter));
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRegimeTributario([FromBody] CreateRegimeTributarioRequest request)
        {
            var response = await mediator.Send(new CreateRegimeTributarioCommand { CreateRegimeTributarioRequest = request });
            return Ok(response);
        }

    }
}
