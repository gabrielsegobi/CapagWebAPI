using System.Text.Json.Nodes;
using Domain.Contracts.CapagFco;
using Domain.Entities;

namespace Application.Helpers
{
    public static class CapagFcoParametrosHelper
    {
        public const string BlocoL100 = "l100";
        public const string BlocoL300 = "l300";
        public const int MaxVersaoBaseLength = 16;
        public const string EmptyJsonObject = "{}";

        public static bool IsValidBloco(string? bloco)
        {
            var normalized = NormalizeBloco(bloco);
            return normalized is BlocoL100 or BlocoL300;
        }

        public static string NormalizeBloco(string? bloco) =>
            (bloco ?? string.Empty).Trim().ToLowerInvariant();

        public static JsonObject ParseExcecoes(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new JsonObject();

            try
            {
                var node = JsonNode.Parse(json);
                return node as JsonObject ?? new JsonObject();
            }
            catch (System.Text.Json.JsonException)
            {
                return new JsonObject();
            }
        }

        /// <summary>
        /// Trim nas chaves (código da conta), ignora chave vazia e valor nulo.
        /// Não interpreta os campos (grupoDfc, trat, acao, parConta, revisar, justificativa).
        /// </summary>
        public static JsonObject NormalizeExcecoes(JsonObject? source)
        {
            var result = new JsonObject();
            if (source == null)
                return result;

            foreach (var kv in source)
            {
                var code = (kv.Key ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(code) || kv.Value is null)
                    continue;

                result[code] = kv.Value.DeepClone();
            }

            return result;
        }

        public static string SerializeExcecoes(JsonObject? excecoes)
        {
            if (excecoes == null || excecoes.Count == 0)
                return EmptyJsonObject;

            return excecoes.ToJsonString();
        }

        public static string? NormalizeVersaoBase(string? versaoBase)
        {
            if (string.IsNullOrWhiteSpace(versaoBase))
                return null;

            var trimmed = versaoBase.Trim();
            return trimmed.Length == 0 ? null : trimmed;
        }

        public static CapagFcoParametrosResponse ToResponse(long idEmpresa, CapagFcoParametroEmpresa? entity)
        {
            var l100 = ParseExcecoes(entity?.ExcecoesL100Json);
            var l300 = ParseExcecoes(entity?.ExcecoesL300Json);

            return new CapagFcoParametrosResponse
            {
                CompanyId = idEmpresa,
                ExcecoesL100 = l100,
                ExcecoesL300 = l300,
                BaseVersaoHash = entity?.BaseVersaoHash,
                UpdatedAt = entity?.UpdatedAt,
                ContasPersonalizadasL100 = l100.Count,
                ContasPersonalizadasL300 = l300.Count
            };
        }
    }
}
