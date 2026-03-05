using System.Text.Json.Serialization;

namespace Domain.Contracts.OperationFiles
{
    public class OperationFilesDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("id_op")]
        public long IdOp { get; set; }
        [JsonPropertyName("file_name")]
        public string FileName { get; set; } = string.Empty;
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }
}
