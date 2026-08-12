using Domain.Contracts.CodigosRegistroDescricao;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.CodigosRegistroDescricao
{
    public class UpdateCodigoRegistroDescricaoCommand : IRequest<UpdateApiResponse>
    {
        public UpdateCodigoRegistroDescricaoCommand(UpdateCodigoRegistroDescricaoRequest request, long id)
        {
            Request = request;
            Id = id;
        }

        public UpdateCodigoRegistroDescricaoRequest Request { get; set; }
        public long Id { get; set; }
    }
}
