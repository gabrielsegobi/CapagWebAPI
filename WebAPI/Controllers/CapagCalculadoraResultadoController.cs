using Application.Commands.CapagCalculadoraResultados;
using Application.Filters;
using Application.Queries.CapagCalculadoraResultados;
using Domain.Contracts.CapagCalculadoraResultados;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CapagCalculadoraResultadoController : BaseApiController
    {
        public CapagCalculadoraResultadoController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCapagCalculadoraResultadoRequest request)
        {
            var response = await mediator.Send(new CreateCapagCalculadoraResultadoCommand(request));
            return Created("", response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CapagCalculadoraResultadoFilter filter)
        {
            var response = await mediator.Send(new GetAllCapagCalculadoraResultadosQuery(filter));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var response = await mediator.Send(new GetCapagCalculadoraResultadoByIdQuery { Id = id });
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateCapagCalculadoraResultadoRequest request, long id)
        {
            var response = await mediator.Send(new UpdateCapagCalculadoraResultadoCommand(request, id));
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await mediator.Send(new DeleteCapagCalculadoraResultadoCommand(id));
            return Ok(response);
        }
    }
}
