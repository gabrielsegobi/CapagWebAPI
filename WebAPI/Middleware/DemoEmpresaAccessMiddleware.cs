using Application.Exceptions.Base;
using Infrastructure.Interface;
using System.Text.Json;

namespace WebAPI.Middleware
{
    /// <summary>
    /// Bloqueia acesso a empresa por id (rota/query) quando o usuário demo
    /// não tem o CNPJ raiz na lista permitida.
    /// </summary>
    public class DemoEmpresaAccessMiddleware
    {
        private static readonly string[] EmpresaIdKeys =
        [
            "idEmpresa",
            "id_empresa",
            "IdEmpresa"
        ];

        private readonly RequestDelegate _next;

        public DemoEmpresaAccessMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IDemoEmpresaAccessService demoAccess)
        {
            if (!demoAccess.IsDemoUser)
            {
                await _next(context);
                return;
            }

            var idEmpresa = ResolveIdEmpresa(context);
            if (idEmpresa.HasValue)
            {
                try
                {
                    await demoAccess.EnsureCanAccessEmpresaAsync(idEmpresa.Value, context.RequestAborted);
                }
                catch (AppException ex)
                {
                    context.Response.StatusCode = ex.StatusCode;
                    context.Response.ContentType = "application/json";

                    var payload = new
                    {
                        error = new
                        {
                            status_code = ex.StatusCode,
                            code = ex.ErrorCode,
                            message = ex.Message,
                            errors = (IEnumerable<string>?)null
                        }
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(payload, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                    }));
                    return;
                }
            }

            await _next(context);
        }

        private static long? ResolveIdEmpresa(HttpContext context)
        {
            foreach (var key in EmpresaIdKeys)
            {
                if (context.Request.RouteValues.TryGetValue(key, out var routeVal)
                    && long.TryParse(routeVal?.ToString(), out var routeId))
                    return routeId;
            }

            if (context.Request.RouteValues.TryGetValue("id", out var idVal)
                && long.TryParse(idVal?.ToString(), out var id)
                && IsEmpresasResourcePath(context.Request.Path))
            {
                return id;
            }

            foreach (var key in EmpresaIdKeys)
            {
                if (context.Request.Query.TryGetValue(key, out var queryVal)
                    && long.TryParse(queryVal.FirstOrDefault(), out var queryId))
                    return queryId;
            }

            return null;
        }

        private static bool IsEmpresasResourcePath(PathString path)
        {
            var value = path.Value ?? string.Empty;
            return value.Contains("/empresas/", StringComparison.OrdinalIgnoreCase)
                   || value.EndsWith("/empresas", StringComparison.OrdinalIgnoreCase);
        }
    }
}
