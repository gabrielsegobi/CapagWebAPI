using Application.Commands.Empresas;
using Application.Filters;
using Application.Queries.DemonstrativosContabeis;
using Application.Queries.Empresas;
using Domain.Contracts.Empresas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmpresasController : BaseApiController
    {
        public EmpresasController(IMediator mediator) : base(mediator) { }

        [HttpPost]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> CreateEmpresa([FromBody] CreateEmpresaRequest request)
        {
            var response = await mediator.Send(new CreateEmpresaCommand { CreateEmpresaRequest = request });
            return Ok(response);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetEmpresaById(long id)
        {
            var response = await mediator.Send(new GetEmpresaByIdQuery { Id = id });
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmpresas([FromQuery] EmpresaFilter filter)
        {
            var response = await mediator.Send(new GetAllEmpresasQuery(filter));
            return Ok(response);
        }



        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> UpdateEmpresa(long id, [FromBody] UpdateEmpresaRequest request)
        {
            var response = await mediator.Send(new UpdateEmpresaCommand
            {
                EmpresaId = id,
                UpdateEmpresaRequest = request
            });

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> DeleteEmpresa(long id)
        {
            var response = await mediator.Send(new DeleteEmpresaCommand { IdEmpresa = id });
            return Ok(response);
        }

        [HttpGet("demonstrativos-contabeis")]
        public async Task<IActionResult> GetDemonstrativosContabeis([FromQuery] long idEmpresa, [FromQuery] bool ano)
        {
            var query = new GetDClByAnoAndCodigoQuery
            {
                IdEmpresa = idEmpresa,
                Ano = ano
            };

            var resultado = await mediator.Send(query);
            return Ok(resultado);
        }
    }
}
