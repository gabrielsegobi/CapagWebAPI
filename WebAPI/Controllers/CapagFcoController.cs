using Application.Commands.CapagFco;
using Application.Queries.CapagFco;
using Domain.Contracts.CapagFco;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Exceções FCO (L100/L300) por empresa. O catálogo universal permanece no client.
    /// Paths consumidos pelo client-app via REACT_APP_API_URL / proxy BFF.
    /// </summary>
    [ApiController]
    [Route("api/empresas/{idEmpresa:long}/capag-fco")]
    [Authorize]
    public class CapagFcoController : BaseApiController
    {
        public CapagFcoController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>GET .../parametros — exceções L100/L300 da empresa (vazio = 100% default).</summary>
        [HttpGet("parametros")]
        public async Task<IActionResult> GetParametros(long idEmpresa)
        {
            var response = await mediator.Send(new GetCapagFcoParametrosQuery { IdEmpresa = idEmpresa });
            return Ok(response);
        }

        /// <summary>PUT .../parametros — substitui as exceções de um bloco (l100 ou l300).</summary>
        [HttpPut("parametros")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> UpsertParametros(
            long idEmpresa,
            [FromBody] UpsertCapagFcoParametrosRequest request)
        {
            var response = await mediator.Send(new UpsertCapagFcoParametrosCommand
            {
                IdEmpresa = idEmpresa,
                Request = request
            });
            return Ok(response);
        }
    }
}
