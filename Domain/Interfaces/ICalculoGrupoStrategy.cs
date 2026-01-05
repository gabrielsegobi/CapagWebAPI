using Domain.Contracts.Responses;

namespace Domain.Interfaces
{
    public interface ICalculoGrupoStrategy
    {
        string Tag { get; }
        Task<GetApiResponse>  CalcularAsync(long idEmpresa, string ano ,CancellationToken cancellationToken);
    }
}
