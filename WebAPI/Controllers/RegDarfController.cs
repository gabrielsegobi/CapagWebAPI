using Application.Commands.RegDarf;
using Application.Filters;
using Application.Queries.RegDarf;
using Domain.Contracts.RegDarf;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RegDarfController : BaseApiController
    {
        public RegDarfController(IMediator mediator) : base(mediator) { }

        [HttpPost]
        public async Task<IActionResult> CreateRegDarf([FromBody] CreateRegDarfRequest request)
        {
            var response = await mediator.Send(new CreateRegDarfCommand(request.FileName, request.Type, request.Requests));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRegDarfById(int id)
        {
            var response = await mediator.Send(new GetRegDarfByIdQuery(id));
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRegsDarf([FromQuery] RegDarfFilter filter)
        {
            var response = await mediator.Send(new GetAllRegsDarfQuery(filter));
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRegDarf(int id, [FromBody] UpdateRegDarfRequest request)
        {
            var response = await mediator.Send(new UpdateRegDarfCommand(request, id));

            return Ok(response);
        }
    }
}
