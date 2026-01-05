using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly IMediator mediator;

        public BaseApiController(IMediator mediator)
        {
            this.mediator = mediator;
        }
    }
}
