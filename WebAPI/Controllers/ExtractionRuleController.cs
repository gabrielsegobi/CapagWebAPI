using Application.Commands.ExtractionRules;
using Application.Filters;
using Application.Queries.ExtractionRules;
using Domain.Contracts.ExtractionRules;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExtractionRuleController : BaseApiController
    {
        public ExtractionRuleController(IMediator mediator) : base(mediator) { }


        [HttpPost]
        public async Task<IActionResult> CreateExtractionRule([FromBody] CreateExtractionRuleRequest request)
        {
            var response = await mediator.Send(new CreateExtractionRuleCommand(request));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExtractionRuleById(int id)
        {
            var response = await mediator.Send(new GetExtractionRuleByIdQuery(id));
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetExtractionRules([FromQuery] ExtractionRuleFilter filter)
        {
            var response = await mediator.Send(new GetAllExtractionRulesQuery(filter));
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExtractionRule(int id, [FromBody] UpdateExtractionRuleRequest request)
        {
            var response = await mediator.Send(new UpdateExtractionRuleCommand(id, request));

            return Ok(response);
        }
    }
}
