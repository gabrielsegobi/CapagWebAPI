using Application.Filters;
using Application.Queries.ModelosIndicesICP;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ModelosIndicesICPController : BaseApiController
    {
        public ModelosIndicesICPController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAllModelosIndicesICP([FromQuery] ModeloIndiceICPFilter filter)
        {
            var response = await mediator.Send(new GetAllModelosIndicesICPQuery(filter));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetModeloIndiceICPById(long id)
        {
            var response = await mediator.Send(new GetModeloIndiceICPByIdQuery { Id = id });
            return Ok(response);
        }

    }
}
