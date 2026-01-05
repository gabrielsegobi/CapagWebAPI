using Application.Commands.UsuariosTenants;
using Application.Filters;
using Application.Queries.UsuariosTenants;
using Domain.Contracts.UsuarioTenant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuariosTenantController : BaseApiController
    {
        public UsuariosTenantController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsuariosTenants([FromQuery] UsuarioTenantFilter filter)
        {
            var response = await mediator.Send(new GetAllUsuariosTenantQuery(filter));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuarioTenantById(long id)
        {
            var response = await mediator.Send(new GetUsuarioTenantByIdQuery(id));
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUsuarioTenant([FromBody] CreateUsuarioTenantRequest request)
        {
            var response = await mediator.Send(new CreateUsuarioTenantCommand(request));
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuarioTenant(long id, [FromBody] UpdateUsuarioTenantRequest request)
        {
            var response = await mediator.Send(new UpdateUsuarioTenantCommand(id, request));
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuarioTenant(long id)
        {
            var response = await mediator.Send(new DeleteUsuarioTenantCommand(id));
            return Ok(response);
        }
    }
}
