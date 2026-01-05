using Domain.Contracts.DemonstrativosContabeis;
using Domain.Contracts.RegimesTributarios;
using Domain.Entities;

namespace Application
{
    public interface IIntegracaoDemonstrativosService
    {
        Task<List<CreateDRERequest>> ObterDreAsync(Empresa empresa, CancellationToken cancellationToken);
        Task<List<CreateBalancoRequest>> ObterBalancoAsync(Empresa empresa, CancellationToken cancellationToken);
        Task<List<CreateRegimeTributarioRequest>> ObterTributacoesAsync(Empresa empresa, CancellationToken cancellationToken);
    }
}
