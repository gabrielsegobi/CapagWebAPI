namespace Application.Filters
{
    public class ValorCalcVariavelFilter : BaseFilter
    {
        public long? IdEmpresa { get; set; }
        public long? IdTipoGrupo { get; set; }
        public int? AnoBase { get; set; }
        public string? IdVariavel { get; set; }
        public decimal? Valor { get; set; }
        public string? Status { get; set; }
    }
}
