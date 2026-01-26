using Application.Commands.RegDefis;
using Application.Filters;
using Application.Queries.RegDefis;
using Domain.Contracts.RegDefis;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RegDefisController : BaseApiController
    {
        public RegDefisController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRegDefisRequest request)
        {
            var response = await mediator.Send(new CreateRegDefisCommand(request));

            return Ok(response);
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] RegDefisFilter filter)
        {
            var response = await mediator.Send(new GetAllRegDefisQuery(filter));

            return Ok(response);
        }
    }
}
