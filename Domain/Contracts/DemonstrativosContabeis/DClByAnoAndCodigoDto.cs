namespace Domain.Contracts.DemonstrativosContabeis
{
    public class DClByAnoAndCodigoDto
    {
        public string Codigo { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public int? Ano { get; set; } 
    }
}
