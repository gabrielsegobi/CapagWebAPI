using Application.Commands.ICPLimits;
using Application.Filters;
using Application.Queries.ICPLimits;
using Domain.Contracts.ICPLimits;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ICPLimitsController : BaseApiController
    {
        public ICPLimitsController(IMediator mediator) : base(mediator) { }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdaICPLimit(long id, [FromBody] UpdateICPLimitRequest request)
        {
            var response = await mediator.Send(new UpdateICPLimitCommand
            {
                Id = id,
                UpdateICPLimitRequest = request
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetICPLimitById(long id)
        {
            var response = await mediator.Send(new GetICPLimitByIdQuery { Id = id });
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmpresas([FromQuery] ICPLimitFilter filter)
        {
            var response = await mediator.Send(new GetAllICPLimitsQuery(filter));
            return Ok(response);
        }

    }
}
