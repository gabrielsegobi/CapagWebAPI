using Domain.Contracts.Json;
using Infrastructure.Interface;
using System.Net.Http.Json;

namespace Infrastructure.Services
{
    public class ApiDadosService : IApiDadosService
    {
        private readonly HttpClient _httpClient;

        public ApiDadosService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<JsonResponse<T>> ObterDadosApiAsync<T>(string url, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro ao buscar dados de API: {response.StatusCode}");

            var json = await response.Content.ReadFromJsonAsync<JsonResponse<T>>(cancellationToken: cancellationToken);
            if (json?.Data == null || !json.Data.Any())
                throw new Exception($"Nenhum dado retornado da API em {url}");

            return json;
        }
    }
}
