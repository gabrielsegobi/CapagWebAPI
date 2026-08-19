using Application.Interfaces;
using Application.Services.Capag;
using Application.Services.Demonstrativos;
using Application.Services.PrlA;
using Application.Services.SinalContabil;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class SinalContabilServiceCollectionExtensions
    {
        public static IServiceCollection AddSinalContabilServices(this IServiceCollection services)
        {
            services.AddSingleton<IGrupoContabilResolver, GrupoContabilResolver>();
            services.AddSingleton<INormalizadorSinalService, NormalizadorSinalService>();
            services.AddSingleton<LucroBrutoCalculator>();
            services.AddSingleton<ResultadoLiquidoCalculator>();
            services.AddSingleton<DemonstrativoArvoreBuilder>();

            services.AddScoped<DemonstrativoLeituraService>();
            services.AddScoped<DemonstrativoConsultaService>();
            services.AddScoped<IDreCapagStructureProvider, DreCapagStructureProvider>();
            services.AddScoped<GreBuilderService>();
            services.AddScoped<IPrlAService, PrlAService>();

            return services;
        }
    }
}
