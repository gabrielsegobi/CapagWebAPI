using Application.Commands.ValorCalcVariaveis;
using Application.Filters;
using Application.Queries.ValorCalcVariaveis;
using Domain.Contracts.ValorCalcVariaveis;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ValorCalcVariavelController : BaseApiController
    {
        public ValorCalcVariavelController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateValorCalcVariavelRequest request)
        {
            var response = await mediator.Send(new CreateValorCalcVariavelCommand(request));
            return Created("", response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ValorCalcVariavelFilter filter)
        {
            var response = await mediator.Send(new GetAllValorCalcVariavelQuery(filter));
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateValorCalcVariavelRequest request, long id)
        {
            var response = await mediator.Send(new UpdateValorCalcVariavelCommand(id, request));
            return Ok(response);
        }
    }
}

