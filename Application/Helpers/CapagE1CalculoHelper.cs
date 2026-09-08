using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Constants;
using Domain.Contracts.CapagE1;
using Domain.Entities;

namespace Application.Helpers
{
    public static class CapagE1CalculoHelper
    {
        public static readonly JsonSerializerOptions JsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };

        public static string NormalizeModelo(string? modelo)
        {
            var value = string.IsNullOrWhiteSpace(modelo)
                ? CapagCalculadoraModelos.CapagE1
                : modelo.Trim();
            return value;
        }

        public static bool IsValidModelo(string? modelo)
        {
            var value = NormalizeModelo(modelo);
            return CapagCalculadoraModelos.Todos.Contains(value);
        }

        public static string SerializePayload(CapagE1CalculoPayload payload) =>
            JsonSerializer.Serialize(payload, JsonOptions);

        public static CapagE1CalculoPayload DeserializePayload(string json) =>
            JsonSerializer.Deserialize<CapagE1CalculoPayload>(json, JsonOptions)
            ?? new CapagE1CalculoPayload();

        /// <summary>
        /// Clona o payload e injeta id / id_empresa / modelo / atualizado_em canônicos.
        /// </summary>
        public static CapagE1CalculoPayload PrepareForStorage(
            CapagE1CalculoPayload request,
            long idEmpresa,
            string modelo,
            long? id,
            DateTime atualizadoEm)
        {
            var copy = DeserializePayload(SerializePayload(request));
            copy.Id = id;
            copy.IdEmpresa = idEmpresa;
            copy.Modelo = modelo;
            copy.AtualizadoEm = DateTime.SpecifyKind(atualizadoEm, DateTimeKind.Unspecified);
            return copy;
        }

        public static CapagE1CalculoPayload ToResponse(CapagE1Calculo entity)
        {
            var payload = DeserializePayload(entity.PayloadJson);
            payload.Id = entity.Id;
            payload.IdEmpresa = entity.IdEmpresa;
            payload.Modelo = entity.Modelo;
            payload.AtualizadoEm = DateTime.SpecifyKind(entity.DateUpdate, DateTimeKind.Unspecified);
            return payload;
        }
    }
}
