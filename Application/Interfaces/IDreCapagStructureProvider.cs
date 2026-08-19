using Domain.Contracts.Demonstrativos;
using Domain.Entities;

namespace Application.Interfaces
{
    public class DreCapagEstrutura
    {
        public long EmpresaId { get; set; }
        public List<int> Anos { get; set; } = new();
        public List<LinhaDemonstrativoDto> Linhas { get; set; } = new();
        public List<ContaAnoSaldoRaw> ContasFolha { get; set; } = new();
        public HashSet<string> ContasExcluidas { get; set; } = new(StringComparer.Ordinal);
    }

    public class ContaAnoSaldoRaw
    {
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal ValorNormalizado { get; set; }
        public decimal SaldoBruto { get; set; }
        public char? Indicador { get; set; }
        public int Ano { get; set; }
    }

    public interface IDreCapagStructureProvider
    {
        Task<DreCapagEstrutura> ObterEstruturaDreCapag(long empresaId, CancellationToken cancellationToken = default);
    }
}
