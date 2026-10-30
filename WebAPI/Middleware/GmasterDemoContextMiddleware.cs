using Application.Helpers;
using Infrastructure.Interface;

namespace WebAPI.Middleware
{
    /// <summary>
    /// Lê contexto do usuário Gmaster enviado pelo BFF/front:
    /// X-Gmaster-Is-Demo, X-Gmaster-User-Id, X-Gmaster-Demo-Cnpjs.
    /// </summary>
    public class GmasterDemoContextMiddleware
    {
        public const string IsDemoHeader = "X-Gmaster-Is-Demo";
        public const string UserIdHeader = "X-Gmaster-User-Id";
        public const string DemoCnpjsHeader = "X-Gmaster-Demo-Cnpjs";

        private readonly RequestDelegate _next;

        public GmasterDemoContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
        {
            var isDemo = IsTruthy(context.Request.Headers[IsDemoHeader].FirstOrDefault());
            var gmasterUserId = context.Request.Headers[UserIdHeader].FirstOrDefault();
            var demoCnpjsHeader = context.Request.Headers[DemoCnpjsHeader].FirstOrDefault();

            IEnumerable<string>? demoCnpjs = null;
            if (isDemo && !string.IsNullOrWhiteSpace(demoCnpjsHeader))
                demoCnpjs = CnpjRootHelper.ParseRoots([demoCnpjsHeader]);

            currentUserService.SetDemoContext(isDemo, gmasterUserId, demoCnpjs);

            await _next(context);
        }

        private static bool IsTruthy(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return value.Trim().ToLowerInvariant() is "1" or "true" or "yes" or "sim";
        }
    }
}
