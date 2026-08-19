using Application.Commands.PrlA;
using Application.Queries.PrlA;
using Domain.Contracts.PrlA;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/prl-a")]
    [Authorize]
    public class PrlAController : BaseApiController
    {
        public PrlAController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("{empresaId:long}")]
        public async Task<IActionResult> Get(long empresaId, [FromQuery] int? ano)
        {
            var response = await mediator.Send(new GetPrlAQuery
            {
                EmpresaId = empresaId,
                Ano = ano
            });
            return Ok(response);
        }

        [HttpPatch("{empresaId:long}/contas/{codigoConta}/bloco")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> PatchBloco(
            long empresaId,
            string codigoConta,
            [FromBody] PatchBlocoPrlARequest request)
        {
            var response = await mediator.Send(new PatchBlocoPrlACommand
            {
                EmpresaId = empresaId,
                CodigoConta = Uri.UnescapeDataString(codigoConta),
                Request = request
            });
            return Ok(response);
        }

        [HttpPatch("{empresaId:long}/contas/{codigoConta}/desagio")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> PatchDesagio(
            long empresaId,
            string codigoConta,
            [FromBody] PatchDesagioPrlARequest request)
        {
            var response = await mediator.Send(new PatchDesagioPrlACommand
            {
                EmpresaId = empresaId,
                CodigoConta = Uri.UnescapeDataString(codigoConta),
                Request = request
            });
            return Ok(response);
        }
    }
}
