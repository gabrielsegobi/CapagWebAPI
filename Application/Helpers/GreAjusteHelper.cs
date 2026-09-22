using System.Text.Json;
using Domain.Contracts.Capag;
using Domain.Entities;

namespace Application.Helpers
{
    public static class GreAjusteHelper
    {
        public const string TipoReceita = "receita";
        public const string TipoDespesa = "despesa";

        public static readonly string[] Tipos = [TipoReceita, TipoDespesa];

        public static bool IsTipoValido(string? tipo) =>
            !string.IsNullOrWhiteSpace(tipo)
            && Tipos.Contains(tipo.Trim().ToLowerInvariant());

        public static string NormalizarTipo(string? tipo)
        {
            var value = tipo?.Trim().ToLowerInvariant();
            return IsTipoValido(value) ? value! : TipoReceita;
        }

        public static string GerarCodigoManual() =>
            $"MAN.{Guid.NewGuid():N}"[..16];

        public static Dictionary<int, decimal> ParseValores(string? json)
        {
            if (string.IsNullOrWhiteSpace(json) || json == "{}")
                return new Dictionary<int, decimal>();

            try
            {
                return JsonSerializer.Deserialize<Dictionary<int, decimal>>(json)
                    ?? new Dictionary<int, decimal>();
            }
            catch (JsonException)
            {
                return new Dictionary<int, decimal>();
            }
        }

        public static string SerializeValores(Dictionary<int, decimal>? valores) =>
            JsonSerializer.Serialize(valores ?? new Dictionary<int, decimal>());

        public static ContaGreManualDto ToDto(ContaGreManual entity) => new()
        {
            Id = entity.Id,
            CodigoConta = entity.CodigoConta,
            CodigoPai = entity.CodigoPai,
            Descricao = entity.Descricao,
            Tipo = entity.Tipo,
            UsarMedia = entity.UsarMedia,
            Justificativa = entity.Justificativa,
            Valores = ParseValores(entity.ValoresJson)
        };

        public static decimal AplicarInversao(decimal valor, bool invertido) =>
            invertido ? -valor : valor;

        public static Dictionary<int, decimal> ResolverValoresManuais(
            Dictionary<int, decimal> valores,
            IReadOnlyCollection<int> anos,
            bool usarMedia)
        {
            var resultado = new Dictionary<int, decimal>();
            if (usarMedia && valores.Count > 0)
            {
                var media = Math.Round(valores.Values.Average(), 2, MidpointRounding.AwayFromZero);
                foreach (var ano in anos)
                    resultado[ano] = media;
                return resultado;
            }

            foreach (var ano in anos)
                resultado[ano] = valores.TryGetValue(ano, out var v) ? v : 0m;

            return resultado;
        }

        public static string? PrimeiraJustificativa(params string?[] valores) =>
            valores.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
    }
}
