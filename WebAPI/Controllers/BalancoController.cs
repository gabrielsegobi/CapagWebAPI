using Application.Queries.Demonstrativos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/balanco")]
    [Authorize]
    public class BalancoController : BaseApiController
    {
        public BalancoController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("{empresaId:long}")]
        public async Task<IActionResult> Get(long empresaId)
        {
            var response = await mediator.Send(new GetBalancoQuery { EmpresaId = empresaId });
            return Ok(response);
        }
    }
}
