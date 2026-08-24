namespace Application.Filters
{
    public class CapagCalculadoraResultadoFilter : BaseFilter
    {
        public CapagCalculadoraResultadoFilter()
        {
            Sort = "datecreate";
            OrderByDescending = true;
        }

        public long? IdEmpresa { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public string Classificacao { get; set; } = string.Empty;
        public bool? Parcial { get; set; }
    }
}
