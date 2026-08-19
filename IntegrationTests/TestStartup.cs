using Application;
using Application.Handlers.ValorCalcVariaveis;
using Application.Mapping;
using Infrastructure.Context;
using Infrastructure.Interface;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using System.IO;

namespace IntegrationTests
{
    public static class TestStartup
    {
        public static IServiceProvider Provider()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            var connectionString = "server=192.168.0.184;port=3306;database=gsaas;user=user;password=password; Persist Security Info=false;";

            // Configurar DbContext com MySQL
            services.AddDbContext<CPGDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            // Configuração do IConfiguration
            //var configuration = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json")
            //    .Build();

            //var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateValorCalcVariavelHandler>());

            // Configurar DbContext
            services.AddDbContext<CPGDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // Registrar repositórios
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddSinalContabilServices();

            //// Registrar AutoMapper
            //var mappingConfig = new MapperConfiguration(cfg =>
            //{
            //    cfg.AddProfile<MappingProfile>();
            //});
            //IMapper mapper = mappingConfig.CreateMapper();
            //services.AddSingleton(mapper);

            // Registrar Handler
            services.AddScoped<CreateValorCalcVariavelHandler>();

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            // Registrar ICurrentUserService (mock para teste)
            services.AddScoped<ICurrentUserService, FakeCurrentUserService>();

            return services.BuildServiceProvider();
        }
    }

    // Mock simples para teste
    public class FakeCurrentUserService : ICurrentUserService
    {
        public int UserId => 1;
        public string? UserName => "TestUser";

        public string? Email => throw new NotImplementedException();

        public string? Role => throw new NotImplementedException();

        public long? TenantId => 1;

        public bool IsAuthenticated => throw new NotImplementedException();

        long? ICurrentUserService.UserId => UserId;

        public IDictionary<string, string> GetAllClaims()
        {
            throw new NotImplementedException();
        }

        public string? GetClaimValue(string claimType)
        {
            throw new NotImplementedException();
        }

        public void SetRole(string? role)
        {
            throw new NotImplementedException();
        }

        public void SetTenantId(long? tenantId)
        {
            throw new NotImplementedException();
        }
    }
}
