using Domain.Contracts.Indicadores;
using MediatR;

namespace Application.Commands.Indicadores
{
    public class CreateIndicadorCommand: IRequest
    {
        public CreateIndicadorRequest CreateIndicadorRequest = new();
    }
}
