using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Domain.Contracts.CapagFco
{
    public class UpsertCapagFcoParametrosRequest
    {
        /// <summary>Bloco a persistir: l100 ou l300.</summary>
        [JsonPropertyName("bloco")]
        public string Bloco { get; set; } = string.Empty;

        /// <summary>
        /// Mapa conta → campos que diferem do catálogo.
        /// Null é rejeitado; objeto vazio limpa as exceções do bloco.
        /// </summary>
        [JsonPropertyName("excecoes")]
        public JsonObject? Excecoes { get; set; }

        /// <summary>Hash da versão do catálogo no client (até 16 caracteres).</summary>
        [JsonPropertyName("versaoBase")]
        public string? VersaoBase { get; set; }
    }
}
