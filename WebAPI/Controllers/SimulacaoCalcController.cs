using Application.Commands.SimulacoesCalc;
using Application.Filters;
using Application.Queries.SimulacoesCalc;
using Domain.Contracts.SimulacoesCalc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SimulacaoCalcController : BaseApiController
    {
        public SimulacaoCalcController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSimulacaoCalcRequest request)
        {
            var response = await mediator.Send(new CreateSimulacaoCalcCommand(request));
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SimulacaoCalcFilter filter)
        {
            var response = await mediator.Send(new GetAllSimulacoesCalcQuery(filter));
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateSimulacaoCalcRequest request, long id)
        {
            var response = await mediator.Send(new UpdateSimulacaoCalcCommand(id, request));
            return Ok(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await mediator.Send(new DeleteSimulacaoCalcCommand(id));
            return Ok(response);
        }
    }
}
