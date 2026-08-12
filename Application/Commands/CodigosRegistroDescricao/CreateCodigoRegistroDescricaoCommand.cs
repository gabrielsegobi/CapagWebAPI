using Domain.Contracts.CodigosRegistroDescricao;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.CodigosRegistroDescricao
{
    public class CreateCodigoRegistroDescricaoCommand : IRequest<CreateApiResponse>
    {
        public CreateCodigoRegistroDescricaoCommand(CreateCodigoRegistroDescricaoRequest request)
        {
            Request = request;
        }

        public CreateCodigoRegistroDescricaoRequest Request { get; set; }
    }
}
