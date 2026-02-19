using Application.Commands.SPED.ECF;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities;

namespace WebAPI.Controllers.SPED
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ECFController : BaseApiController
    {
        public ECFController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("upload")]
        //[Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadECF([FromForm] UploadECFRequest request)
        {
            //if (file == null || file.Length == 0)
            //    return BadRequest("Arquivo inválido.");

            var command = new UploadECFCommand(request);


            await mediator.Send(command);
            return Ok("Arquivo enviado com sucesso!");
        }

        [HttpPut("reprocess/{id_empresa}")]
        public async Task<IActionResult> ReprocessEcf(long id_empresa)
        {

            var command = new ReprocessEcfCommand(id_empresa);
            await mediator.Send(command);

            return Ok("Arquvios com erro foram marcados para serem reprocessados");
        }
    }
}
