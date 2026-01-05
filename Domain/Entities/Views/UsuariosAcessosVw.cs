namespace Domain.Entities.Views
{
    public class UsuariosAcessosVw
    {
        public string StatusTenant { get; set; } = string.Empty;
        public string SlugTenant { get; set; } = string.Empty;
        public string Papel { get; set; } = string.Empty;
        public string NomeUsuario { get; set; } = string.Empty;
        public string NomeTenant { get; set; } = string.Empty;
        public long IdUsuario { get; set; } 
        public long IdTenant { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
