namespace Application.Filters
{
    public class CarteiraEmpresaFilter : BaseFilter
    {
        public string? NomeEmpresa { get; set; }
        public string? Cnpj { get; set; }
        public string? Status { get; set; }
        public DateTime? DataImpedimento { get; set; }
        public string? StatusBloqueio { get; set; }
        public string? RatingCapag { get; set; }
        public int? UltimoAnoEcf { get; set; }
        public DateTime? DataCalculo { get; set; }
    }
}
