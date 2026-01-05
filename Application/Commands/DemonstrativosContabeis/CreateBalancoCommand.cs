using Application.Mediator;
using Domain.Contracts.DemonstrativosContabeis;
using MediatR;

namespace Application.Commands.DemonstrativosContabeis
{
    public class CreateBalancoCommand : IRequest
    {
        public List<CreateBalancoRequest>? CreateBalancoRequests { get; set; }
    }
}
