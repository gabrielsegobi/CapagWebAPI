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
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> CreateRegIrpf([FromBody] CreateRegIrpfRequest request)
        {
            var response = await mediator.Send(new CreateRegIrpfCommand(request));

            return Created("", response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRegsIrpf([FromQuery] RegIrpfFilter filter)
        {
            var response = await mediator.Send(new GetAllRegsIrpfQuery(filter));
            return Ok(response);
        }

        [HttpGet("/count/{id_filename}")]
        public async Task<IActionResult> Count(long id_filename)
        {
            var response = await mediator.Send(new RegIrpfCountQuery(id_filename));
            return Ok(response);
        }
    }
}
