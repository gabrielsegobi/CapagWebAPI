namespace Domain.Entities
{
    public class ProcessLog : ITenantEntity
    {
        public ProcessLog(long idEmpresa, string acao, string mensagem)
        {
            IdEmpresa = idEmpresa;
            Acao = acao;
            CreatedAt = GetDateTimeNow();
            Mensagem = mensagem;
        }

        public long Id { get; set; }
        public long IdEmpresa { get; set; }
        public string Acao { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public long IdTenant { get; set; }

        private DateTime GetDateTimeNow()
        {
            var now = DateTime.UtcNow;
            return now.AddHours(-3);
        }

    }
}
