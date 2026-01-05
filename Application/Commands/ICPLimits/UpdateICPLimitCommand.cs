using Domain.Contracts.ICPLimits;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.ICPLimits
{
    public class UpdateICPLimitCommand: IRequest<UpdateApiResponse>
    {
        public long Id { get; set; }
        public UpdateICPLimitRequest UpdateICPLimitRequest { get; set; } = new UpdateICPLimitRequest();
    }
}
