using Domain.Interfaces;

namespace Application.Factories
{
    public interface ICalculoGrupoFactory
    {
        ICalculoGrupoStrategy ObterPorTag(string tag);

    }
}
