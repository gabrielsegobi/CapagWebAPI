using Application.Filters;
using Application.Queries.AnalisesICP;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnalisesICPController : BaseApiController
    {
        public AnalisesICPController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAllAnalisesICP([FromQuery] AnaliseICPFilter filter)
        {
            var response = await mediator.Send(new GetAllAnalisesICPQuery(filter));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnaliseById(long id)
        {
            var response = await mediator.Send(new GetAnaliseICPByIdQuery { Id = id });
            return Ok(response);
        }

        [HttpGet("relatorio")]
        public async Task<IActionResult> GetRelatorio()
        {
            var response = await mediator.Send(new GetAnaliseIcpRelatorioQuery());
            return Ok(response);
        }

    }
}
