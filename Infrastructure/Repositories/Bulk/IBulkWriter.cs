using Domain.Entities.Sped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Bulk
{
    public interface IBulkWriter : IAsyncDisposable
    {
        /// <summary>
        /// Adiciona uma linha ao batch interno.
        /// Quando o batch atingir o tamanho configurado, flush automático.
        /// </summary>
        Task AddAsync(ParsedRow row, CancellationToken cancellationToken = default);

        /// <summary>
        /// Força o envio de todos os batches pendentes para o banco.
        /// Sempre chame no final do processamento.
        /// </summary>
        Task FlushAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Métricas de escrita — útil para log e diagnóstico.
        /// </summary>
        BulkWriterMetrics Metrics { get; }
    }

    public record BulkWriterMetrics
    {
        public long TotalLinhasInseridas { get; init; }
        public int TotalFlushes { get; init; }
        public long TempoTotalFlushMs { get; init; }
    }
}
