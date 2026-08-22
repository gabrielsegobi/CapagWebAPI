using Application.Commands.Carteira;
using Application.Filters;
using Application.Queries.Carteira;
using Domain.Contracts.Carteira;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/carteira")]
    [Authorize]
    public class CarteiraController : BaseApiController
    {
        public CarteiraController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// Lista empresas da carteira comercial com paginação e filtros.
        /// DataImpedimento filtra o dia exato; DataImpedimentoAte filtra até o dia informado (inclusive).
        /// </summary>
        [HttpGet("empresas")]
        public async Task<IActionResult> GetEmpresas([FromQuery] CarteiraEmpresaFilter filter)
        {
            var response = await mediator.Send(new GetCarteiraEmpresasQuery(filter));
            return Ok(response);
        }

        /// <summary>
        /// Atualiza status comercial, valor de contrato e data de impedimento de uma empresa.
        /// </summary>
        [HttpPut("empresas/{id:long}")]
        [Authorize(Roles = "Admin,editor")]
        public async Task<IActionResult> AtualizarEmpresa(long id, [FromBody] AtualizarCarteiraEmpresaRequest request)
        {
            var response = await mediator.Send(new AtualizarCarteiraEmpresaCommand
            {
                EmpresaId = id,
                Request = request
            });
            return Ok(response);
        }

        /// <summary>
        /// Retorna a distribuição de ratings por mês no intervalo informado.
        /// Parâmetros mesDe e mesAte no formato MM/yyyy (ex: 02/2026).
        /// </summary>
        [HttpGet("rating")]
        public async Task<IActionResult> GetRating(
            [FromQuery] string? mesDe,
            [FromQuery] string? mesAte)
        {
            var response = await mediator.Send(new GetCarteiraRatingQuery
            {
                MesDe = mesDe,
                MesAte = mesAte
            });
            return Ok(response);
        }

        /// <summary>
        /// Retorna o painel resumido de contratos (contagens e volumes financeiros).
        /// </summary>
        [HttpGet("contratos")]
        public async Task<IActionResult> GetContratos()
        {
            var response = await mediator.Send(new GetPainelContratosQuery());
            return Ok(response);
        }
    }
}
