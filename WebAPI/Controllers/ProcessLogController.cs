using Application.Commands.ProcessLog;
using Application.Filters;
using Application.Queries.ProcessLog;
using Domain.Contracts.ProcessLog;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProcessLogController : BaseApiController
    {
        public ProcessLogController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProcessLogs([FromQuery] ProcessLogFilter filter)
        {
            var response = await mediator.Send(new GetAllProcessLogQuery(filter));
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProcessLog([FromBody] CreateProcessLogRequest request)
        {
            var response = await mediator.Send(new CreateProcessLogCommand { CreateProcessLogRequest = request });
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProcessLog(long id)
        {
            var response = await mediator.Send(new DeleteProcessLogCommand(id));
            return Ok(response);
        }
    }
}

