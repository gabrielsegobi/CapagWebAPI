using Domain.Contracts.ICPAnterior;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.ICPAnterior
{
    public class UpdateICPAnteriorCommand : IRequest<UpdateApiResponse>
    {
        public UpdateICPAnteriorCommand(UpdateICPAnteriorRequest request, long id)
        {
            Request = request;
            Id = id;
        }

        public UpdateICPAnteriorRequest Request { get; set; }
        public long Id { get; set; }
    }
}
