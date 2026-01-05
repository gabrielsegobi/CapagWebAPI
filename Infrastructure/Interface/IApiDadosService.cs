using Domain.Contracts.Json;

namespace Infrastructure.Interface
{
    public interface IApiDadosService
    {
        Task<JsonResponse<T>> ObterDadosApiAsync<T>(string url, CancellationToken cancellationToken);
    }
}
