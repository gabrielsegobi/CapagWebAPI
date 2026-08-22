using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Helpers
{
    /// <summary>
    /// Serializa o pipeline de uma empresa via GET_LOCK do MySQL (named lock).
    /// Evita dois jobs apagarem/inserirem demonstrativos ao mesmo tempo.
    /// </summary>
    public static class EmpresaProcessamentoLock
    {
        public static string Nome(long idEmpresa) => $"capag_proc_empresa_{idEmpresa}";

        /// <summary>
        /// Abre a conexão do DbContext, adquire o lock e mantém a conexão aberta até Dispose.
        /// timeoutSeconds = 0 → falha imediata se outro job já segura o lock.
        /// </summary>
        public static async Task<IAsyncDisposable> AdquirirAsync(
            DatabaseFacade database,
            long idEmpresa,
            CancellationToken cancellationToken,
            int timeoutSeconds = 0)
        {
            await database.OpenConnectionAsync(cancellationToken);
            var connection = database.GetDbConnection();
            var nome = Nome(idEmpresa);

            var adquirido = await ExecutarLockAsync(connection, "SELECT GET_LOCK(@nome, @timeout)", nome, timeoutSeconds, cancellationToken);
            if (adquirido != 1)
            {
                await database.CloseConnectionAsync();
                throw new InvalidOperationException(
                    $"Já existe processamento em andamento para a empresa {idEmpresa} (GET_LOCK={adquirido}).");
            }

            return new Releaser(database, connection, nome);
        }

        private static async Task<int?> ExecutarLockAsync(
            DbConnection connection,
            string sql,
            string nome,
            int? timeoutSeconds,
            CancellationToken cancellationToken)
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            var pNome = cmd.CreateParameter();
            pNome.ParameterName = "@nome";
            pNome.Value = nome;
            cmd.Parameters.Add(pNome);

            if (timeoutSeconds.HasValue)
            {
                var pTimeout = cmd.CreateParameter();
                pTimeout.ParameterName = "@timeout";
                pTimeout.Value = timeoutSeconds.Value;
                cmd.Parameters.Add(pTimeout);
            }

            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            if (result is null or DBNull)
                return null;

            return Convert.ToInt32(result);
        }

        private sealed class Releaser : IAsyncDisposable
        {
            private readonly DatabaseFacade _database;
            private readonly DbConnection _connection;
            private readonly string _nome;
            private bool _disposed;

            public Releaser(DatabaseFacade database, DbConnection connection, string nome)
            {
                _database = database;
                _connection = connection;
                _nome = nome;
            }

            public async ValueTask DisposeAsync()
            {
                if (_disposed)
                    return;

                _disposed = true;
                try
                {
                    if (_connection.State == ConnectionState.Open)
                        await ExecutarLockAsync(_connection, "SELECT RELEASE_LOCK(@nome)", _nome, null, CancellationToken.None);
                }
                catch
                {
                    // Liberação best-effort; conexão pode já ter caído.
                }
                finally
                {
                    try
                    {
                        await _database.CloseConnectionAsync();
                    }
                    catch
                    {
                        // ignore
                    }
                }
            }
        }
    }
}
