using System.Text.Json.Serialization;

namespace Domain.Contracts.Empresas
{
    public class UpdateEmpresaImpedimentoRequest
    {
        [JsonPropertyName("data_impedimento")]
        public DateTime DataImpedimento { get; set; }

        [JsonPropertyName("id_usuario_responsavel")]
        public long IdUsuarioResponsavel { get; set; }
    }
}
