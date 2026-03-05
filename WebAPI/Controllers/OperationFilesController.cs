using Application.Commands.OperationFiles;
using Application.Filters;
using Application.Queries.OperationsFiles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OperationFilesController : BaseApiController
    {
        public OperationFilesController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] OperationFilesFilter filter)
        {
            var result = await mediator.Send(new GetAllOperationFilesQuery(filter));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await mediator.Send(new DeleteOperationFileCommand(id));
            return Ok(result);
        }
    }
}