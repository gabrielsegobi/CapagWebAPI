using Application.Queries.Demonstrativos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/dre")]
    [Authorize]
    public class DreController : BaseApiController
    {
        public DreController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("{empresaId:long}")]
        public async Task<IActionResult> Get(long empresaId)
        {
            var response = await mediator.Send(new GetDreQuery { EmpresaId = empresaId });
            return Ok(response);
        }
    }
}
