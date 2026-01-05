using Application.Commands.RegsPgdasd;
using Application.Filters;
using Application.Queries.RegsPgdasd;
using Domain.Contracts.RegsPgdasd;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RegPgdasdController : BaseApiController
    {
        public RegPgdasdController(IMediator mediator) : base(mediator) { }

        [HttpPost]
        public async Task<IActionResult> CreateRegPgdasd([FromBody] CreateRegPgdasdRequest request)
        {
            var response = await mediator.Send(
                new CreateRegPgdasdCommand(
                    request.FileName,
                    request.Type,
                    request.Requests
                )
            );

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRegsPgdasd([FromQuery] RegPgdasdFilter filter)
        {
            var response = await mediator.Send(new GetAllRegsPgdasdQuery(filter));
            return Ok(response);
        }
    }
}
