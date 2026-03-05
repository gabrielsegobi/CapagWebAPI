using Domain.Entities.Sped;
using Domain.Interfaces;
using Domain.Resources.Sped;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Infrastructure
{
    /// <summary>
    /// Extension method para registrar todos os serviços SPED no DI.
    /// Uso no Program.cs: builder.Services.AddSpedServices(builder.Configuration);
    /// </summary>
    public static class SpedServiceExtensions
    {
        public static IServiceCollection AddSpedServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 1. Lê e valida a configuração
            var settings = configuration
                .GetSection(SpedLayoutSettings.Section)
                .Get<SpedLayoutSettings>()
                ?? throw new InvalidOperationException(
                    $"Seção '{SpedLayoutSettings.Section}' não encontrada no appsettings.json. " +
                    $"Adicione:\n" +
                    $"  \"SpedLayouts\": {{\n" +
                    $"    \"Ecf\": \"Resources/Sped/Ecf\"\n" +
                    $"  }}");

            // 2. Resolve o caminho absoluto em runtime
            // AppContext.BaseDirectory = pasta onde o .dll está rodando
            // Em dev:        bin/Debug/net8.0/
            // Em produção:   /app/ ou C:/inetpub/wwwroot/
            // Nunca usa C:\Users\... — funciona em qualquer ambiente
            var caminhoEcf = Path.Combine(
                AppContext.BaseDirectory,
                settings.Ecf.Replace('/', Path.DirectorySeparatorChar));

            // 3. Valida que a pasta existe antes de registrar
            // Melhor falhar no startup do que na primeira requisição
            if (!Directory.Exists(caminhoEcf))
                throw new DirectoryNotFoundException(
                    $"Pasta de layouts ECF não encontrada: {caminhoEcf}\n" +
                    $"Verifique se os JSONs estão em '{settings.Ecf}' " +
                    $"e se o .csproj tem CopyToOutputDirectory configurado.");

            // 4. Registra como Singleton — lê disco 1x, cacheia para sempre
            services.AddSingleton<ILayoutRepository>(
                new JsonLayoutRepository(caminhoEcf));

            return services;
        }
    }
}
