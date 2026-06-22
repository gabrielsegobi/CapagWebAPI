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

        [HttpGet("contas")]
        public async Task<IActionResult> GetContas([FromQuery] long? id_empresa)
        {
            if (!Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader) ||
                !long.TryParse(tenantHeader, out var idTenant))
                return BadRequest("Tenant não informado.");

            var response = await mediator.Send(new GetContasContabeisQuery
            {
                IdTenant = idTenant,
                IdEmpresa = id_empresa
            });

            return Ok(response);
        }

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

        [HttpPost("cadastrar")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> Cadastrar([FromBody] CadastrarDemonstrativosContabeisRequest request)
        {
            if (!Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader) ||
                !long.TryParse(tenantHeader, out var idTenant))
                return BadRequest("Tenant não informado.");

            if (request?.Contas == null || request.Contas.Count == 0)
                return BadRequest("Lista de contas inválida.");

            var response = await mediator.Send(new CadastrarDemonstrativosContabeisCommand
            {
                IdTenant = idTenant,
                Sobrescrever = request.Sobrescrever,
                Contas = request.Contas
            });

            if (!response.Sucesso)
                return BadRequest(response);

            return Ok(response);
        }
    }
}

