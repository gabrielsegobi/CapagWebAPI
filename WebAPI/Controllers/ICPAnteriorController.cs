using Application.Commands.ICPAnterior;
using Application.Filters;
using Application.Queries.ICPAnterior;
using Domain.Contracts.ICPAnterior;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ICPAnteriorController : BaseApiController
    {
        public ICPAnteriorController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ICPAnteriorFilter filter)
        {
            var reponse = await mediator.Send(new GetAllICPAnteriorQuery(filter));
            return Ok(reponse);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateICPAnteriorRequest request)
        {
            var reponse = await mediator.Send(new CreateICPAnteriorCommand(request));
            return Ok(reponse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, UpdateICPAnteriorRequest request)
        {
            var reponse = await mediator.Send(new UpdateICPAnteriorCommand(request, id));
            return Ok(reponse);
        }
    }
}
