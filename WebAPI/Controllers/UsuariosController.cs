using Application.Commands.Usuarios;
using Application.Filters;
using Application.Queries.Usuarios;
using Domain.Contracts.Usuarios;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuariosController : BaseApiController
    {
        public UsuariosController(IMediator mediator) : base(mediator) { }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUsuario([FromBody] CreateUsuarioRequest request)
        {
            var response = await mediator.Send(new CreateUsuarioCommand { CreateUsuarioRequest = request });

            return CreatedAtAction(
                   nameof(GetUsuarioById),
                   new { id = response.Id },
                   response
               );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuarioById(long id)
        {
            var response = await mediator.Send(new GetUsuarioByIdQuery { Id = id });
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsuarios([FromQuery] UsuarioFilter filter)
        {
            var response = await mediator.Send(new GetAllUsuariosQuery(filter));
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(long id, [FromBody] UpdateUsuarioRequest request)
        {
            var response = await mediator.Send(new UpdateUsuarioCommand
            {
                Id = id,
                UpdateUsuarioRequest = request
            });

            return Ok(response);
        }
        [HttpPut("trocar-senha/{id}")]
        public async Task<IActionResult> UpdateSenha(long id, [FromBody] ChangePasswordRequest request)
        {
            var response = await mediator.Send(new ChangePasswordCommand
            {
                Id = id,
                ChangePasswordRequest = request
            });

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(long id)
        {
            var response = await mediator.Send(new DeleteUsuarioCommand { Id = id });
            return Ok(response);
        }

        [HttpGet("tenants")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserTenants()
        {
            var response = await mediator.Send(new GetUserTenantsQuery());
            return Ok(response);
        }

        [HttpGet("tenant-permission")]
        public async Task<IActionResult> GetTenantPermissions()
        {
            var response = await mediator.Send(new GetTenantPermissionsQuery());
            return Ok(response);
        }
    }
}
