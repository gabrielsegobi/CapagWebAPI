using Domain.Interfaces;

namespace Domain.Factories
{
    public class EcfFactory : IEcfFactory
    {
        // strategies vindas do DI
        private readonly IDictionary<string, IEcfBuilderStrategy> _strategies;

        // cache local (por REG)
        private readonly Dictionary<string, IEcfBuilderStrategy> _cache = new();

        public EcfFactory(IEnumerable<IEcfBuilderStrategy> strategies)
        {
            _strategies = strategies.ToDictionary(s => s.Codigo);
        }

        public IEcfBuilderStrategy ObterPorReg(string reg)
        {
            // 1️⃣ tenta pegar do cache
            if (_cache.TryGetValue(reg, out var strategy))
                return strategy;

            // 2️⃣ se não existir, busca no dicionário principal
            if (!_strategies.TryGetValue(reg, out strategy))
                throw new Exception(reg);

            // 3️⃣ salva no cache
            _cache[reg] = strategy;

            return strategy;
        }
    }
}
