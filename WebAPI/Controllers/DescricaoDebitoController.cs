using Application.Commands.DescricaoDebitos;
using Application.Filters;
using Application.Queries.DescricaoDebitos;
using Domain.Contracts.DescricaoDebitos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DescricaoDebitoController : BaseApiController
    {
        public DescricaoDebitoController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDescricaoDebitoRequest request)
        {
            var response = await mediator.Send(new CreateDescricaoDebitoCommand(request));
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DescricaoDebitoFilter filter)
        {
            var response = await mediator.Send(new GetAllDescricoesDebitosQuery(filter));
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateDescricaoDebitoRequest request, long id)
        {
            var response = await mediator.Send(new UpdateDescricaoDebitoCommand(request, id));
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await mediator.Send(new DeleteDescricaoDebitoCommand(id));
            return Ok(response);
        }
    }
}
