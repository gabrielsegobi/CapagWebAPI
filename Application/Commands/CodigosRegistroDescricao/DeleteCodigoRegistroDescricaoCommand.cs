using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.CodigosRegistroDescricao
{
    public class DeleteCodigoRegistroDescricaoCommand : IRequest<DeleteApiResponse>
    {
        public DeleteCodigoRegistroDescricaoCommand(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
