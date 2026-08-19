using System.Text.Json.Serialization;

namespace Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BlocoLiquidez
    {
        A = 0,
        B = 1,
        C = 2
    }
}
