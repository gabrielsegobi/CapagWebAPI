namespace Application.Filters
{
    public class CarteiraEmpresaFilter : BaseFilter
    {
        public string? NomeEmpresa { get; set; }
        public string? Cnpj { get; set; }
        public string? Status { get; set; }

        /// <summary>
        /// Empresas com data de impedimento no dia informado (início inclusive, dia seguinte exclusive).
        /// </summary>
        public DateTime? DataImpedimento { get; set; }

        /// <summary>
        /// Empresas com data de impedimento até o dia informado (inclusive).
        /// </summary>
        public DateTime? DataImpedimentoAte { get; set; }

        public string? StatusBloqueio { get; set; }
        public string? RatingCapag { get; set; }
        public int? UltimoAnoEcf { get; set; }
        public DateTime? DataCalculo { get; set; }
    }
}
