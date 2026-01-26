using Application.Commands.RegsDctf;
using Application.Filters;
using Application.Queries.RegsDctf;
using Domain.Contracts.RegsDctf;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RegDctfController : BaseApiController
    {
        public RegDctfController(IMediator mediator) : base(mediator) { }

        [HttpPost]
        public async Task<IActionResult> CreateRegDctf([FromBody] CreateRegDctfRequest request)
        {
            var response = await mediator.Send(new CreateRegDctfCommand(request));
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRegsDctf([FromQuery] RegDctfFilter filter)
        {
            var response = await mediator.Send(new GetAllRegsDctfQuery(filter));
            return Ok(response);
        }
    }
}
