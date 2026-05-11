using System.Linq;

namespace Domain.Contracts.DemonstrativosContabeis
{
    public class ConstruirDemonstrativosResponse
    {
        public bool Sucesso { get; set; }
        public int TotalInseridos { get; set; }
        public bool PossuiAlertas => Resultados.Values.Any(r => r.Alertas.Count > 0);
        public Dictionary<int, ConstruirDemonstrativosAnoResultado> Resultados { get; set; } = new();
    }

    public class ConstruirDemonstrativosAnoResultado
    {
        public int Inseridos { get; set; }
        public int Deletados { get; set; }
        public List<string> Erros { get; set; } = new();
        public List<string> Alertas { get; set; } = new();
    }
}

