using Application.Commands.Tenants;
using Application.Filters;
using Application.Queries.Tenants;
using Domain.Contracts.Tenants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TenantsController : BaseApiController
    {
        public TenantsController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTenants([FromQuery] TenantFilter filter)
        {
            var response = await mediator.Send(new GetAllTenantsQuery(filter));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTenantById(long id)
        {
            var response = await mediator.Send(new GetTenantByIdQuery { Id = id });
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantRequest request)
        {
            var response = await mediator.Send(new CreateTenantCommand (request));
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTenant(long id, [FromBody] UpdateTenantRequest request)
        {
            var response = await mediator.Send(new UpdateTenantCommand(id, request));
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTenant(long id)
        {
            var response = await mediator.Send(new DeleteTenantCommand (id));
            return Ok(response);
        }
    }
}
