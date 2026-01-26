namespace Domain.Entities
{
    public class ValorCalcVariavel : ITenantEntity
    {
        public long IdValorCalcVariavel { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public long IdTipoGrupo { get; set; }
        public short AnoBase { get; set; }
        public string IdVariavel { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
