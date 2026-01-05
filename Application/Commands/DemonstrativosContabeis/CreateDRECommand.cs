using Domain.Contracts.DemonstrativosContabeis;
using MediatR;

namespace Application.Commands.DemonstrativosContabeis
{
    public class CreateDRECommand: IRequest
    {
        public List<CreateDRERequest>? CreateDRERequest { get; set; }
    }
}
