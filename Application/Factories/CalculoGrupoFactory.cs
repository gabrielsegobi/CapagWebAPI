using Application.Exceptions.TipoGrupo;
using Domain.Interfaces;

namespace Application.Factories;

public class CalculoGrupoFactory : ICalculoGrupoFactory
{
    private readonly IDictionary<string, ICalculoGrupoStrategy> _strategies;

    public CalculoGrupoFactory(IEnumerable<ICalculoGrupoStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.Tag);
    }

    public ICalculoGrupoStrategy ObterPorTag(string tag)
    {
        if (!_strategies.TryGetValue(tag, out var strategy))
            throw new CalculoNotFoundException(tag);

        return strategy;
    }
}
