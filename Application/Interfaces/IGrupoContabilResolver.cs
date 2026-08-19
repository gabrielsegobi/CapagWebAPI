using Domain.Enums;

namespace Application.Interfaces
{
    public interface IGrupoContabilResolver
    {
        GrupoContabil Resolver(string? codigo);
    }
}
