namespace Domain.Entities
{
    public class UsuarioTenant
    {
        public long Id { get; set; }
        public long IdUsuario { get; set; }
        public long IdTenant { get; set; }
        public string Papel { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public DateTime DataVinculo { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
