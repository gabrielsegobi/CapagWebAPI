namespace Domain.Entities
{
    public class RefreshToken
    {
        public long IdRefreshToken { get; set; }
        public long IdUsuario { get; set; }
        public string Token { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public DateTime ExpiraEm { get; set; }
        public DateTime? RevogadoEm { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
