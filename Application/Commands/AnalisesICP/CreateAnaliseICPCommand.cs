using Application.Mediator;
using Domain.Contracts.AnalisesICP;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.AnalisesICP
{
    public class CreateAnaliseICPCommand: IRequest
    {
        public CreateAnaliseICPRequest CreateAnaliseICPRequest = new();
    }
}
