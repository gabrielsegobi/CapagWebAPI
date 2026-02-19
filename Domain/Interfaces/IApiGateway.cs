namespace Domain.Interfaces
{
    public interface IApiGateway
    {
        Task<T> ObterDadosApiAsync<T>(string url, CancellationToken cancellationToken);
    }
}
