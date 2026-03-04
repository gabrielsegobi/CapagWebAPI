using Application.Commands.RegsFileName;
using Application.Queries.RegFileNames;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RegFileNameController : BaseApiController
    {
        public RegFileNameController(IMediator mediator) : base(mediator)
        {
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await mediator.Send(new DeleteRegFileNameCommnad(id));

            return Ok(result);
        }

        [HttpGet("{id_empresa}")]
        public async Task<IActionResult> GetAllByEmpresa(long id_empresa)
        {
            var result = await mediator.Send(new GetAllFilesByEmpresaQuery(id_empresa));
            return Ok(result);

        }
    }
}
