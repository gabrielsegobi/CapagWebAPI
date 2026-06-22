namespace Domain.Contracts.DemonstrativosContabeis
{
    public class CadastrarDemonstrativosContabeisResponse
    {
        public bool Sucesso { get; set; }
        public long IdEmpresa { get; set; }
        public int TotalInseridos { get; set; }
        public int TotalDeletados { get; set; }
        public string? Erro { get; set; }
    }
}
