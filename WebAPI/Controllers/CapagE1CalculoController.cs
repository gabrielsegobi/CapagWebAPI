using Application.Commands.CapagE1;
using Application.Queries.CapagE1;
using Domain.Contracts.CapagE1;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Persistência do estado completo do cálculo CAPAG-e1 (GRE + inversões + resultado).
    /// Paths consumidos pelo client-app quando NEXT_PUBLIC_CAPAG_E1_API_ENABLED=true.
    /// </summary>
    [ApiController]
    [Route("api/capag-e1/calculo")]
    [Authorize]
    public class CapagE1CalculoController : BaseApiController
    {
        public CapagE1CalculoController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>GET /api/capag-e1/calculo?IdEmpresa={id}</summary>
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] long IdEmpresa,
            [FromQuery] string? modelo = null)
        {
            var response = await mediator.Send(new GetCapagE1CalculoQuery
            {
                IdEmpresa = IdEmpresa,
                Modelo = modelo
            });
            return Ok(response);
        }

        /// <summary>POST /api/capag-e1/calculo — cria snapshot; devolve payload com id.</summary>
        [HttpPost]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> Create([FromBody] CapagE1CalculoPayload request)
        {
            var response = await mediator.Send(new CreateCapagE1CalculoCommand(request));
            return Created("", response);
        }

        /// <summary>PUT /api/capag-e1/calculo/{id} — atualiza snapshot intacto.</summary>
        [HttpPut("{id:long}")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> Update(long id, [FromBody] CapagE1CalculoPayload request)
        {
            var response = await mediator.Send(new UpdateCapagE1CalculoCommand(request, id));
            return Ok(response);
        }
    }
}
