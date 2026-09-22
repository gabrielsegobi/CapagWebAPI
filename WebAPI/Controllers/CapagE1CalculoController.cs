using Application.Commands.CapagE1;
using Application.Helpers;
using Application.Queries.CapagE1;
using Domain.Contracts.CapagE1;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Snapshot do cálculo CAPAG (GRE/PLRA/ajustes/resultado).
    /// e1 e e2 usam a mesma tabela/handlers; o path define o modelo
    /// (capag-e-1 vs capag-e-2) para não misturar os documentos.
    /// </summary>
    [ApiController]
    [Route("api/capag-e1/calculo")]
    [Route("api/capag-e2/calculo")]
    [Authorize]
    public class CapagE1CalculoController : BaseApiController
    {
        public CapagE1CalculoController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>GET /api/capag-e1/calculo?IdEmpresa={id} (ou /api/capag-e2/calculo)</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] long IdEmpresa)
        {
            var response = await mediator.Send(new GetCapagE1CalculoQuery
            {
                IdEmpresa = IdEmpresa,
                Modelo = ModeloDaRota()
            });
            return Ok(response);
        }

        /// <summary>POST — cria snapshot; devolve payload com id.</summary>
        [HttpPost]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> Create([FromBody] CapagE1CalculoPayload request)
        {
            request.Modelo = ModeloDaRota();
            var response = await mediator.Send(new CreateCapagE1CalculoCommand(request));
            return Created("", response);
        }

        /// <summary>PUT /{id} — atualiza snapshot intacto.</summary>
        [HttpPut("{id:long}")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> Update(long id, [FromBody] CapagE1CalculoPayload request)
        {
            request.Modelo = ModeloDaRota();
            var response = await mediator.Send(new UpdateCapagE1CalculoCommand(request, id));
            return Ok(response);
        }

        private string ModeloDaRota() =>
            CapagE1CalculoHelper.ModeloDaRota(HttpContext.Request.Path.Value);
    }
}
