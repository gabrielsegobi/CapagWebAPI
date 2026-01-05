using Application.Commands.RegsIrpf;
using Application.Filters;
using Application.Queries.RegsIrpf;
using Domain.Contracts.RegIrpf;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RegIrpfController : BaseApiController
    {
        public RegIrpfController(IMediator mediator) : base(mediator) { }
        [HttpPost]
        public async Task<IActionResult> CreateRegIrpf(
    [FromBody] CreateRegIrpfRequest request)
        {
            var response = await mediator.Send(
                new CreateRegIrpfCommand(
                    request.FileName,
                    request.Type,
                    request.Requests
                )
            );

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRegsIrpf([FromQuery] RegIrpfFilter filter)
        {
            var response = await mediator.Send(new GetAllRegsIrpfQuery(filter));
            return Ok(response);
        }
    }
}
