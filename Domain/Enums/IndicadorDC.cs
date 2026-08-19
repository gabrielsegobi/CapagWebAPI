using System.Text.Json.Serialization;

namespace Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum IndicadorDC
    {
        Debito = 0,
        Credito = 1
    }
}
