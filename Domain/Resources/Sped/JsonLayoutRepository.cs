using Domain.Entities;
using Domain.Entities.Sped.Ecf;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Domain.Resources.Sped
{
    /// <summary>
    /// Implementação do ILayoutRepository que lê arquivos JSON do disco.
    /// Responsabilidade: ler JSONs de uma pasta, deserializar e cachear em memória.
    /// Não sabe nada sobre parsing ou banco.
    ///
    /// Estrutura esperada de pastas:
    ///   /layouts/ecf/P100.json
    ///   /layouts/ecf/P030.json
    ///   /layouts/ecf/L001.json
    ///   ...
    /// </summary>
    public class JsonLayoutRepository : ILayoutRepository
    {
        private readonly string _pastaLayouts;

        // Cache em memória — carregado uma única vez
        private Dictionary<string, EcfLayout>? _cache;
        private readonly SemaphoreSlim _lock = new(1, 1);

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true  // aceita "nome" e "Nome" no JSON
        };

        /// <param name="pastaLayouts">
        /// Caminho da pasta onde estão os arquivos JSON de layout.
        /// Ex: "C:/app/layouts/ecf" ou em produção via IConfiguration
        /// </param>
        public JsonLayoutRepository(string pastaLayouts)
        {
            _pastaLayouts = pastaLayouts;
        }

        public async Task<IEnumerable<EcfLayout>> LoadAllAsync()
        {
            var cache = await GetOrBuildCacheAsync();
            return cache.Values;
        }

        public async Task<EcfLayout?> LoadAsync(string registro)
        {
            var cache = await GetOrBuildCacheAsync();
            return cache.TryGetValue(registro.ToUpper(), out var layout) ? layout : null;
        }

        /// <summary>
        /// Constrói o cache na primeira chamada.
        /// Thread-safe via SemaphoreSlim.
        /// </summary>
        private async Task<Dictionary<string, EcfLayout>> GetOrBuildCacheAsync()
        {
            if (_cache is not null) return _cache;

            await _lock.WaitAsync();
            try
            {
                // Double-check após adquirir o lock
                if (_cache is not null) return _cache;

                _cache = await BuildCacheAsync();
                return _cache;
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task<Dictionary<string, EcfLayout>> BuildCacheAsync()
        {
            if (!Directory.Exists(_pastaLayouts))
                throw new DirectoryNotFoundException(
                    $"Pasta de layouts não encontrada: {_pastaLayouts}");

            var arquivos = Directory.GetFiles(_pastaLayouts, "*.json", SearchOption.AllDirectories);

            if (arquivos.Length == 0)
                throw new InvalidOperationException(
                    $"Nenhum arquivo JSON encontrado em: {_pastaLayouts}");

            var cache = new Dictionary<string, EcfLayout>(
                arquivos.Length,
                StringComparer.OrdinalIgnoreCase);

            foreach (var arquivo in arquivos)
            {
                try
                {
                    await using var stream = File.OpenRead(arquivo);
                    var layout = await JsonSerializer.DeserializeAsync<EcfLayout>(
                        stream, _jsonOptions);

                    if(layout != null)
                    {
                        if (layout.Tabela != "ecf_0000" && layout.Tabela != "ecf_0001" && layout.Tabela != "ecf_0010" && layout.Tabela != "ecf_l001"
                            && layout.Tabela != "ecf_l030" && layout.Tabela != "ecf_l100")
                            continue;

                        //if (layout.Tabela != "ecf_0001")
                        //    continue;
                    }
                    

                    if (layout is null)
                    {
                        Console.WriteLine($"[WARN] Layout nulo ao deserializar: {arquivo}");
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(layout.Registro))
                    {
                        Console.WriteLine($"[WARN] Layout sem campo 'registro': {arquivo}");
                        continue;
                    }

                    cache[layout.Registro.ToUpper()] = layout;
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException(
                        $"JSON inválido no arquivo {arquivo}: {ex.Message}", ex);
                }
            }

            return cache;
        }
    }
}
