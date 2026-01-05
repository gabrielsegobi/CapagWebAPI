using Domain.Resources;

namespace Application.Filters
{
    public class IndicadorFilter : BaseFilter
    {
        public long IdEmpresa { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
    }
}
