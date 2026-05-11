using Application.Commands.DemonstrativosContabeis;
using Application.Queries.DemonstrativosContabeis;
using Domain.Contracts.DemonstrativosContabeis;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/demonstrativos")]
    [Authorize]
    public class DemonstrativosController : BaseApiController
    {
        public DemonstrativosController(IMediator mediator) : base(mediator) { }

        [HttpGet("anos-disponiveis")]
        public async Task<IActionResult> GetAnosDisponiveis([FromQuery] long id_empresa)
        {
            if (!Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader) ||
                !long.TryParse(tenantHeader, out var idTenant))
                return BadRequest("Tenant não informado.");

            var response = await mediator.Send(new GetAnosDisponiveisQuery
            {
                IdTenant = idTenant,
                IdEmpresa = id_empresa
            });

            return Ok(response);
        }

        [HttpPost("construir")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> Construir([FromBody] ConstruirDemonstrativosRequest request)
        {
            if (!Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader) ||
                !long.TryParse(tenantHeader, out var idTenant))
                return BadRequest("Tenant não informado.");

            // Sempre usa tenant do header (fonte de verdade)
            request.IdTenant = idTenant;

            var response = await mediator.Send(new ConstruirDemonstrativosCommand(request));
            return Ok(response);
        }
    }
}

