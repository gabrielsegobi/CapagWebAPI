using Domain.Contracts.RegDarf;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.RegDarf
{
    public class UpdateRegDarfCommand : IRequest<UpdateApiResponse>
    {
        public long Id { get; set; }
        public UpdateRegDarfRequest Request { get; set; }
        public UpdateRegDarfCommand(UpdateRegDarfRequest request, long id)
        {
            Request = request;
            Id = id;
        }
    }
}
