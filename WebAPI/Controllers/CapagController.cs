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

        [HttpPatch("{empresaId:long}/contas/{codigoConta}/inversao")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> PatchInversao(
            long empresaId,
            string codigoConta,
            [FromBody] PatchContaInversaoRequest request)
        {
            var response = await mediator.Send(new PatchContaInversaoCommand
            {
                EmpresaId = empresaId,
                CodigoConta = Uri.UnescapeDataString(codigoConta),
                Request = request
            });
            return Ok(response);
        }

        [HttpPost("{empresaId:long}/contas-manuais")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> CreateContaManual(
            long empresaId,
            [FromBody] UpsertContaGreManualRequest request)
        {
            var response = await mediator.Send(new CreateContaGreManualCommand
            {
                EmpresaId = empresaId,
                Request = request
            });
            return Created("", response);
        }

        [HttpPut("{empresaId:long}/contas-manuais/{id:long}")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> UpdateContaManual(
            long empresaId,
            long id,
            [FromBody] UpsertContaGreManualRequest request)
        {
            var response = await mediator.Send(new UpdateContaGreManualCommand
            {
                EmpresaId = empresaId,
                Id = id,
                Request = request
            });
            return Ok(response);
        }

        [HttpDelete("{empresaId:long}/contas-manuais/{id:long}")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> DeleteContaManual(long empresaId, long id)
        {
            var response = await mediator.Send(new DeleteContaGreManualCommand
            {
                EmpresaId = empresaId,
                Id = id
            });
            return Ok(response);
        }
    }
}
