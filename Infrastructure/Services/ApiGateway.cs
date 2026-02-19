using Domain.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Infrastructure.Services
{
    public class ApiGateway : IApiGateway
    {
        private readonly HttpClient _httpClient;

        public ApiGateway(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", "d6ce29969ebf4d2c3b682519ff257656537c451503069fccd644dfa30d7fe919");
        }

        public async Task<T> ObterDadosApiAsync<T>(string url, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro ao buscar dados de API: {response.StatusCode}");

            var json =
                await response.Content.ReadFromJsonAsync<T>(
                    cancellationToken: cancellationToken);

            if (json is null)
                throw new Exception($"Nenhum dado retornado da API em {url}");

            return json;
        }
    }
}
