using Application.Commands.Capag;
using Application.Queries.Capag;
using Domain.Contracts.Capag;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/capag")]
    [Authorize]
    public class CapagController : BaseApiController
    {
        public CapagController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("{empresaId:long}/gre")]
        public async Task<IActionResult> GetGre(long empresaId)
        {
            var response = await mediator.Send(new GetGreQuery { EmpresaId = empresaId });
            return Ok(response);
        }

        [HttpPatch("{empresaId:long}/contas/{codigoConta}/exclusao")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> PatchExclusao(
            long empresaId,
            string codigoConta,
            [FromBody] PatchContaExclusaoRequest request)
        {
            var response = await mediator.Send(new PatchContaExclusaoCommand
            {
                EmpresaId = empresaId,
                CodigoConta = Uri.UnescapeDataString(codigoConta),
                Request = request
            });
            return Ok(response);
        }
    }
}
