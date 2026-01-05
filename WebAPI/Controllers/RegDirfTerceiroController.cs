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
        public async Task<IActionResult> CreateRegDirf([FromBody] CreateRegDirfTerceiroRequest request)
        {
            var response = await mediator.Send(new CreateRegDirfTerceiroCommand(request.FileName, request.Type, request.Requests));
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRegsDarf([FromQuery] RegDirfTerceiroFilter filter)
        {
            var response = await mediator.Send(new GetAllRegsDirfTerceirosQuery(filter));
            return Ok(response);
        }
    }
}
