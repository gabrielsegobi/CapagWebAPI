using Application.Commands.DocumentLayouts;
using Application.Filters;
using Application.Queries.DocumentLayouts;
using Domain.Contracts.DocumentsLayouts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentLayoutController : BaseApiController
    {
        public DocumentLayoutController(IMediator mediator) : base(mediator) { }


        [HttpPost]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> CreateDocumentLayout([FromBody] CreateDocumentLayoutRequest request)
        {
            var response = await mediator.Send(new CreateDocumentLayoutCommand(request));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumentLayoutById(int id)
        {
            var response = await mediator.Send(new GetDocumentLayoutByIdQuery(id));
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDocumentsLayouts([FromQuery] DocumentLayoutFilter filter)
        {
            var response = await mediator.Send(new GetAllDocumentsLayoutsQuery(filter));
            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> UpdateDocumentLayout(int id, [FromBody] UpdateDocumentLayoutRequest request)
        {
            var response = await mediator.Send(new UpdateDocumentLayoutCommand(id, request));

            return Ok(response);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> DeleteDocumentLayout(int id)
        {
            var response = await mediator.Send(new DeleteDocumentLayoutCommand(id));

            return Ok(response);
        }
    }
}
