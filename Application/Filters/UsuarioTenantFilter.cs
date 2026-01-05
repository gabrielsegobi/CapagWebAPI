namespace Application.Filters
{
    public class UsuarioTenantFilter : BaseFilter
    {
        public long? IdTenant { get; set; }
        public long? IdUsuario { get; set; }
        public string Papel { get; set; } = string.Empty;
        public bool? Ativo { get; set; }
    }
}
