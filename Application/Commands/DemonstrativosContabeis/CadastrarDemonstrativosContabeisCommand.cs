using Domain.Contracts.DemonstrativosContabeis;
using MediatR;

namespace Application.Commands.DemonstrativosContabeis
{
    public class CadastrarDemonstrativosContabeisCommand : IRequest<CadastrarDemonstrativosContabeisResponse>
    {
        public long IdTenant { get; set; }
        public bool Sobrescrever { get; set; }
        public List<CadastrarDemonstrativosContabeisItem> Contas { get; set; } = new();
    }
}
