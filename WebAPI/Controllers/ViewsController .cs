using Application.Filters.Views;
using Application.Queries.Views.BalancoPatrimonial;
using Application.Queries.Views.DRE;
using Domain.Contracts.Responses;
using Domain.Contracts.Views;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ViewsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ViewsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        //[HttpGet("balanco-patrimonial/{id:long}")]
        //public async Task<ActionResult<GetApiResponse>> GetBPViewById(long id)
        //{
        //    var result = await _mediator.Send(new GetBPViewByIdQuery{ Id = id });
        //    return Ok(result);
        //}

        [HttpGet("balanco-patrimonial")]
        public async Task<ActionResult<PagedApiResponse<BPViewDto>>> GetAllBPViews([FromQuery] BPViewFilter filter)
        {
            var result = await _mediator.Send(new GetAllBPViewQuery (filter));
            return Ok(result);
        }

        [HttpGet("dre")]
        public async Task<ActionResult<PagedApiResponse<DREViewDto>>> GetAllDREViews([FromQuery] DREViewFilter filter)
        {
            var result = await _mediator.Send(new GetAllDREViewQuery(filter));
            return Ok(result);
        }

        //[HttpGet("dashboard-empresa-anual/{id:long}")]
        //public async Task<ActionResult<GetApiResponse>> GetDEAViewById(long id)
        //{
        //    var result = await _mediator.Send(new GetDEAViewByIdQuery { Id = id });
        //    return Ok(result);
        //}

        //[HttpGet("DRE/{id:long}")]
        //public async Task<ActionResult<GetApiResponse>> GetDREViewById(long id)
        //{
        //    var result = await _mediator.Send(new GetDREViewByIdQuery { Id = id });
        //    return Ok(result);
        //}

        //[HttpGet("usuarios-acessos/{id:long}")]
        //public async Task<ActionResult<GetApiResponse>> GetUAViewById(long id)
        //{
        //    var result = await _mediator.Send(new GetUAViewByIdQuery { Id = id });
        //    return Ok(result);
        //}
    }
}
