using Application.Commands.RegDirfTerceiros;
using Application.Filters;
using Application.Queries.RegDirfTerceiros;
using Domain.Contracts.RegDirfTerceiros;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RegDirfTerceiroController : BaseApiController
    {
        public RegDirfTerceiroController(IMediator mediator) : base(mediator) { }

        [HttpPost]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> CreateRegDirf([FromBody] CreateRegDirfTerceiroRequest request)
        {
            var response = await mediator.Send(new CreateRegDirfTerceiroCommand(request));
            return Created("", response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRegsDarf([FromQuery] RegDirfTerceiroFilter filter)
        {
            var response = await mediator.Send(new GetAllRegsDirfTerceirosQuery(filter));
            return Ok(response);
        }

        [HttpGet("/count/{id_filename}")]
        public async Task<IActionResult> Count(long id_filename)
        {
            var response = await mediator.Send(new RegDirfCountQuery(id_filename));
            return Ok(response);
        }
    }
}
