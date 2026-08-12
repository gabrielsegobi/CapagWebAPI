using Application.Commands.CodigosRegistroDescricao;
using Application.Filters;
using Application.Queries.CodigosRegistroDescricao;
using Domain.Contracts.CodigosRegistroDescricao;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CodigoRegistroDescricaoController : BaseApiController
    {
        public CodigoRegistroDescricaoController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCodigoRegistroDescricaoRequest request)
        {
            var response = await mediator.Send(new CreateCodigoRegistroDescricaoCommand(request));
            return Created("", response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CodigoRegistroDescricaoFilter filter)
        {
            var response = await mediator.Send(new GetAllCodigosRegistroDescricaoQuery(filter));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var response = await mediator.Send(new GetCodigoRegistroDescricaoByIdQuery { Id = id });
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateCodigoRegistroDescricaoRequest request, long id)
        {
            var response = await mediator.Send(new UpdateCodigoRegistroDescricaoCommand(request, id));
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await mediator.Send(new DeleteCodigoRegistroDescricaoCommand(id));
            return Ok(response);
        }
    }
}
