namespace Application.Filters
{
    public class ModeloIndiceICPFilter: BaseFilter
    {
        public string Nome { get; set; } = string.Empty;
        public string Formula { get; set; } = string.Empty;
        public long? IdEmpresa { get; set; }

    }
}
