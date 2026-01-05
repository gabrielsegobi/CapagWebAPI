using Domain.Entities;

namespace Infrastructure.Interface
{
    public interface ITokenService
    {
        string GerarToken(Usuario usuario, string papel);
    }

}
