using Domain.Contracts.ICPAnterior;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.ICPAnterior
{
    public class CreateICPAnteriorCommand : IRequest<CreateApiResponse>
    {
        public CreateICPAnteriorCommand(CreateICPAnteriorRequest request)
        {
            Request = request;
        }

        public CreateICPAnteriorRequest Request { get; set; }
    }
}
