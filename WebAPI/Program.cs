using API.Middleware;
using Application;
using Application.Behaviors;
using Application.Commands.Usuarios;
using Application.Factories;
using Application.Mapping;
using Application.Strategies;
using Application.teste;
using Domain.Contracts.Views;
using Domain.Entities.Sped;
using Domain.Factories;
using Domain.Interfaces;
using Domain.Strategies.Sped.Ecf;
using FluentValidation;
using Infrastructure;
using Infrastructure.BackgroundJobs;
using Infrastructure.Context;
using Infrastructure.Interface;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // seu frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // se usar cookies ou JWT via header
    });
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<CPGDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped(typeof(IBaseViewRepository<>), typeof(BaseViewRepository<>));

var basePath = builder.Configuration["Storage:BasePath"];

builder.Services.AddSingleton<IObjectStorage>(
    new LocalFileStorage(basePath)
);

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<AssemblyMarker>();
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 2L * 1024 * 1024 * 1024; // 1 GB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 2L * 1024 * 1024 * 1024; // 1 GB
});

//var channel = Channel.CreateBounded<ParsedRow>(
//    new BoundedChannelOptions(10_000)
//    {
//        FullMode = BoundedChannelFullMode.Wait,
//        SingleWriter = false,  
//        SingleReader = true
//    });

//builder.Services.AddSingleton(channel);
//builder.Services.AddSingleton(channel.Reader);
//builder.Services.AddSingleton(channel.Writer);

//builder.Services.AddHostedService<BulkWorker>();


builder.Services.AddValidatorsFromAssembly(typeof(CreateUsuarioCommand).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddScoped<IPasswordHasher, Sha256PasswordHasher>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IIntegracaoDemonstrativosService, IntegracaoDemonstrativosService>();
builder.Services.AddHttpClient<IApiDadosService, ApiDadosService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<AuditInterceptor>();
builder.Services.AddScoped<IEcfFactory, EcfFactory>();
builder.Services.AddScoped<IEcfBuilderStrategy, E_0000Builder>();
builder.Services.AddScoped<IEcfBuilderStrategy, E_0001Builder>();
builder.Services.AddScoped<IEcfBuilderStrategy, E_0010Builder>();
builder.Services.AddScoped<IEcfBuilderStrategy, E_L001Builder>();
builder.Services.AddScoped<IEcfBuilderStrategy, E_L030Builder>();
builder.Services.AddScoped<IEcfBuilderStrategy, E_L100Builder>();
builder.Services.AddScoped<IEcfBuilderStrategy, E_L300Builder>();
builder.Services.AddScoped<IEcfBuilderStrategy, E_P001builder>();
builder.Services.AddScoped<IEcfBuilderStrategy, E_P030Builder>();
builder.Services.AddScoped<IEcfBuilderStrategy, E_P100builder>();
builder.Services.AddScoped<IEcfBuilderStrategy, E_P150builder>();
builder.Services.AddScoped<IEcfProcessorService, EcfProcessorService>();

builder.Services.AddScoped<IBulkInsertService, BulkInsertService>();

//builder.Services.AddScoped<ICalculoGrupoStrategy, PessoaJuridicaCalculoStrategy>();
//builder.Services.AddScoped<ICalculoGrupoStrategy, PessoaFisicaCalculoStrategy>();
builder.Services.AddScoped<ICalculoGrupoStrategy, PjNOptanteCalculoStrategy>();
builder.Services.AddScoped<ICalculoGrupoStrategy, PjOptanteCalculoStrategy>();
builder.Services.AddScoped<IGetRelationShip, GetRelationShip>();
builder.Services.AddScoped<IApiGateway, ApiGateway>();
builder.Services.AddScoped<ICalculoGrupoStrategy, PfCalculoStrategy>();
builder.Services.AddScoped<ICalculoGrupoFactory, CalculoGrupoFactory>();


//builder.Services.AddSingleton<ILayoutRepository, JsonLayoutRepository>();
builder.Services.AddSpedServices(builder.Configuration);



var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Erro de token: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validado com sucesso!");
            return Task.CompletedTask;
        }
    };
});

//builder.Services.AddSwaggerGen(c =>
//{
//    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
//        Scheme = "Bearer",
//        BearerFormat = "JWT",
//        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
//        Description = "Insira o token JWT assim: Bearer {seu token}"
//    });

//    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
//    {
//        {
//            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//            {
//                Reference = new Microsoft.OpenApi.Models.OpenApiReference
//                {
//                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});


builder.Services.AddAuthorization();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddSingleton<EcfBackgroundWorker>();
builder.Services.AddHostedService<QueuedHostedService>();
//builder.Services.AddHostedService<EcfBackgroundWorker>();
builder.Services.AddHostedService(provider =>
    provider.GetRequiredService<EcfBackgroundWorker>());
builder.Services.AddSingleton<EcfReprocessWorker>();
builder.Services.AddHostedService(provider =>
    provider.GetRequiredService<EcfReprocessWorker>());
var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "cApAG"));
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseMiddleware<TenantPermissionMiddleware>();
app.UseMiddleware<AuthorizationMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
