using Domain.Contracts.DemonstrativosContabeis;
using Domain.Contracts.RegimesTributarios;
using Domain.Entities;

namespace Application
{
    public interface IIntegracaoDemonstrativosService
    {
        Task<List<CreateDRERequest>> ObterDreAsync(Empresa empresa, IReadOnlyList<int> anos, CancellationToken cancellationToken);
        Task<List<CreateBalancoRequest>> ObterBalancoAsync(Empresa empresa, IReadOnlyList<int> anos, CancellationToken cancellationToken);
        Task<List<CreateRegimeTributarioRequest>> ObterTributacoesAsync(Empresa empresa, CancellationToken cancellationToken);
    }
}
