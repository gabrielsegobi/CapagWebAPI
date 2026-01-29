using Application.Commands.SimulacoesCalc;
using Application.Commands.SimulacoesIntervalo;
using Application.Filters;
using Application.Queries.SimulacoesCalc;
using Domain.Contracts.SimulacoesCalc;
using Domain.Contracts.SimulacoesIntervalo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SimulacaoIntervaloController : BaseApiController
    {
        public SimulacaoIntervaloController(IMediator mediator) : base(mediator)
        {
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSimulacaoIntervaloRequest request)
        {
            var response = await mediator.Send(new CreateSimulacaoIntervaloCommand(request));
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SimulacaoIntervaloFilter filter)
        {
            var response = await mediator.Send(new GetAllSimulacoesIntervalosQuery(filter));
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateSimulacaoIntervaloRequest request, long id)
        {
            var response = await mediator.Send(new UpdateSimulacaoIntervaloCommand(id, request));
            return Ok(response);
        }
    }
}
