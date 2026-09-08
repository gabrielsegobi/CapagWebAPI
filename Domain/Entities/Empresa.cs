using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Empresa: ITenantEntity
    {
        public long IdEmpresa { get; set; }
        public long IdTenant { get; set; }
        public string Cnpj { get; private set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;
        public string? NomeFantasia { get; set; } = string.Empty;
        public string MatrizFilial { get; set; } = string.Empty;
        public long? IdEmpresaMatriz { get; set; }
        public bool Ativa { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool DadosProcessados { get; set; }
        public string Cnae  { get; set; } = string.Empty;
        public string MunicipioEstado  { get; set; } = string.Empty;
        public DateTime? DataAbertura { get; set; }
        public string CapitalSocial  { get; set; } = string.Empty;
        public string Segmento  { get; set; } = string.Empty;
        public string Porte  { get; set; } = string.Empty;
        public DateTime? DataImpedimento { get; set; }
        public DateTime? DataProtocolo { get; set; }
        public string? Status { get; set; }
        public decimal? ValorContrato { get; set; }
        public long? IdUsuarioResponsavel { get; set; }
    }
}
