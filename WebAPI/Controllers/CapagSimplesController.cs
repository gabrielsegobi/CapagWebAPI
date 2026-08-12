using Application.Commands.CapagSimples;
using Application.Queries.CapagSimples;
using Domain.Contracts.CapagSimples;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Declaração Simples Nacional e valores manuais de DRE/Balanço.
    /// Paths consumidos pelo client-app via apiCapag / proxy_capag.
    /// </summary>
    [ApiController]
    [Route("api/empresas/{idEmpresa:long}/capag-simples")]
    [Authorize]
    public class CapagSimplesController : BaseApiController
    {
        public CapagSimplesController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>GET .../declaration — declaração da empresa ou vazia.</summary>
        [HttpGet("declaration")]
        public async Task<IActionResult> GetDeclaration(long idEmpresa)
        {
            var response = await mediator.Send(new GetSimplesDeclarationQuery { IdEmpresa = idEmpresa });
            return Ok(response);
        }

        /// <summary>PUT .../declaration — upsert declaração + substituição de exercícios.</summary>
        [HttpPut("declaration")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> UpsertDeclaration(
            long idEmpresa,
            [FromBody] UpsertSimplesDeclarationRequest request)
        {
            var response = await mediator.Send(new UpsertSimplesDeclarationCommand
            {
                IdEmpresa = idEmpresa,
                Request = request
            });
            return Ok(response);
        }

        /// <summary>DELETE .../declaration — remove declaração e exercícios (idempotente).</summary>
        [HttpDelete("declaration")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> DeleteDeclaration(long idEmpresa)
        {
            await mediator.Send(new DeleteSimplesDeclarationCommand { IdEmpresa = idEmpresa });
            return Ok();
        }

        /// <summary>
        /// GET .../demonstrative-values — lista valores manuais.
        /// Query opcional: ?kind=DRE e/ou ?kind=BALANCE_SHEET (repetível).
        /// </summary>
        [HttpGet("demonstrative-values")]
        public async Task<IActionResult> GetDemonstrativeValues(
            long idEmpresa,
            [FromQuery] List<string>? kind)
        {
            var response = await mediator.Send(new GetManualDemonstrativeValuesQuery
            {
                IdEmpresa = idEmpresa,
                Kinds = kind
            });
            return Ok(response);
        }

        /// <summary>PUT .../demonstrative-values — replace semântico de um kind inteiro.</summary>
        [HttpPut("demonstrative-values")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> UpsertDemonstrativeValues(
            long idEmpresa,
            [FromBody] UpsertManualDemonstrativeValuesRequest request)
        {
            var response = await mediator.Send(new UpsertManualDemonstrativeValuesCommand
            {
                IdEmpresa = idEmpresa,
                Request = request
            });
            return Ok(response);
        }
    }
}
