using Domain.Entities;
using Domain.Entities.Sped.Ecf;

namespace Domain.Interfaces
{
    /// <summary>
    /// Contrato para carregamento de layouts de registros SPED.
    /// Responsabilidade: abstrair de onde os layouts vêm (JSON, banco, memória).
    /// O caller não sabe se está lendo de arquivo ou cache.
    /// </summary>
    public interface ILayoutRepository
    {
        /// <summary>
        /// Carrega todos os layouts disponíveis.
        /// Deve ser chamado UMA única vez na inicialização — resultado deve ser cacheado.
        /// </summary>
        Task<IEnumerable<EcfLayout>> LoadAllAsync();

        /// <summary>
        /// Carrega o layout de um registro específico.
        /// </summary>
        Task<EcfLayout?> LoadAsync(string registro);
    }
}
