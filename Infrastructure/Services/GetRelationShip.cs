using Domain.Contracts;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class GetRelationShip : IGetRelationShip
    {
        private readonly IConfiguration _configuration;
        private readonly IApiGateway _apiGateway;
        private readonly string _baseUrl;

        public GetRelationShip(IConfiguration configuration, IApiGateway apiGateway)
        {
            _configuration = configuration;
            _apiGateway = apiGateway;
            _baseUrl = _configuration["ApiGmaster:BaseUrl"] ?? throw new ArgumentNullException("ApiGmaster:BaseUrl");
        }

        public async Task<List<Relationship>> ExecuteAsync(string type, CancellationToken cancellationToken)
        {
            var url = $"{_baseUrl}/efd_contrib/relationship?type={type}";
            var response = await _apiGateway.ObterDadosApiAsync<ApiResponse<List<Relationship>>>(url, cancellationToken);
            return response.Data;
        }
    }
}
