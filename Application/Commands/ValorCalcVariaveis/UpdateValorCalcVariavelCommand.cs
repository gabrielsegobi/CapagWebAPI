using Domain.Contracts.Responses;
using Domain.Contracts.ValorCalcVariaveis;
using MediatR;

namespace Application.Commands.ValorCalcVariaveis
{
    public class UpdateValorCalcVariavelCommand : IRequest<UpdateApiResponse>
    {
        public UpdateValorCalcVariavelCommand(long id, UpdateValorCalcVariavelRequest request)
        {
            Id = id;
            Request = request;
        }

        public long Id { get; set; }
        public UpdateValorCalcVariavelRequest Request { get; set; }
    }
}
