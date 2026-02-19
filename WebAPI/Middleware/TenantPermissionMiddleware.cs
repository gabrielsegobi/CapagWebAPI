using Domain.Entities;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WebAPI.Middleware
{
    public class TenantPermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantPermissionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ICurrentUserService currentUserService,
            IBaseRepository<UsuarioTenant> usuarioTenantRepository)
        {
            // 1️⃣ Só roda em rotas autenticadas
            var endpoint = context.GetEndpoint();
            //var requiresAuth = endpoint?.Metadata.GetMetadata<IAuthorizeData>() != null;

            var hasAuthorize =
                endpoint?.Metadata.GetOrderedMetadata<IAuthorizeData>()?.Any() == true;

            var allowAnonymous =
                endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null;

            if (!hasAuthorize || allowAnonymous)
            {
                await _next(context);
                return;
            }
           

            // 2️⃣ Tenant obrigatório
            if (!context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader)
                || !long.TryParse(tenantHeader, out var tenantId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Tenant não informado.");
                return;
            }

            // 3️⃣ UserId já veio do token (validado pelo UseAuthentication)
            var userId = currentUserService.UserId;
            if (!userId.HasValue)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            // 4️⃣ Busca vínculo Usuario × Tenant
            var usuarioTenant = await usuarioTenantRepository.GetFirstOrDefaultAsync(x =>
                  x.IdUsuario == userId.Value &&
                  x.IdTenant == tenantId &&
                  x.Ativo &&
                  x.DeletedAt == null
              );

            if (usuarioTenant == null || !usuarioTenant.Ativo || usuarioTenant.DeletedAt != null)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Usuário não possui acesso a este tenant.");
                return;
            }

            // 5️⃣ Popula contexto
            currentUserService.SetTenantId(tenantId);
            currentUserService.SetRole(usuarioTenant.Papel);

            // (Opcional) adiciona role como claim dinâmica
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, usuarioTenant.Papel)
            });

            context.User.AddIdentity(identity);

            await _next(context);
        }
    }
}
