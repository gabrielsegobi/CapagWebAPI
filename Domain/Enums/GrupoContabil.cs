using System.Text.Json.Serialization;

namespace Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GrupoContabil
    {
        Ativo = 0,
        Passivo = 1,
        PatrimonioLiquido = 2,
        Receita = 3,
        Despesa = 4
    }
}
