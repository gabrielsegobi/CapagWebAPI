namespace Domain.Contracts.DemonstrativosContabeis
{
    public class AnosDisponiveisResponse
    {
        public List<AnoDisponivelItem> Anos { get; set; } = new();
        public int Total { get; set; }
    }

    public class AnoDisponivelItem
    {
        public int Ano { get; set; }
        public int RegistrosDefis { get; set; }
        public int CompetenciasPgdasd { get; set; }
        public bool JaExisteDemonstrativo { get; set; }
    }
}

