using Domain.Entities;
using Domain.Enums;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace Infrastructure
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _http;
        private readonly ICurrentUserService _currentUser;

        public AuditInterceptor(IHttpContextAccessor http, ICurrentUserService currentUser)
        {
            _http = http;
            _currentUser = currentUser;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            AddAuditLogs(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            AddAuditLogs(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void AddAuditLogs(DbContext? context)
        {
            if (context == null) return;


            var entries = context.ChangeTracker.Entries()
                .Where(e =>
                    e.Entity != null &&
                    !(e.Entity is AuditLog) && 
                    (e.State == EntityState.Added ||
                     e.State == EntityState.Modified ||
                     e.State == EntityState.Deleted))
                .ToList();

            if (!entries.Any()) return;

            foreach (var entry in entries)
            {
                try
                {
                    var log = BuildAuditLog(entry);
                    if (log != null)
                    {
                        
                        context.Set<AuditLog>().Add(log);
                    }
                }
                catch
                {
                    // Não deixar a auditoria quebrar a operação principal.
                    // Se quiser, registre em console aqui ou em algum fallback.
                }
            }
        }

        private AuditLog? BuildAuditLog(EntityEntry entry)
        {
            var entityName = entry.Entity.GetType().Name;

            // Primary key (concat if composite)
            var pkValues = entry.Properties
                .Where(p => p.Metadata.IsPrimaryKey())
                .Select(p => p.CurrentValue?.ToString())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

            var idRegistroStr = pkValues.Length > 0 ? string.Join(",", pkValues) : "0";

            // map EF state to AcaoEnum
            var acao = entry.State switch
            {
                EntityState.Added => AcaoEnum.INSERT,
                EntityState.Modified => AcaoEnum.UPDATE,
                EntityState.Deleted => AcaoEnum.DELETE,
                _ => AcaoEnum.INSERT
            };

            string dadosAntigos = "{}";
            string dadosNovos = "{}";

            if (entry.State == EntityState.Modified)
            {
                try
                {
                    var original = entry.OriginalValues.ToObject();
                    dadosAntigos = JsonSerializer.Serialize(original, new JsonSerializerOptions { WriteIndented = false });
                }
                catch
                {
                    dadosAntigos = "{}";
                }
            }
            else if (entry.State == EntityState.Deleted)
            {
                try
                {
                    var original = entry.OriginalValues.ToObject();
                    dadosAntigos = JsonSerializer.Serialize(original, new JsonSerializerOptions { WriteIndented = false });
                }
                catch
                {
                    dadosAntigos = "{}";
                }
            }

            if (entry.State != EntityState.Deleted)
            {
                try
                {
                    var current = entry.CurrentValues.ToObject();
                    dadosNovos = JsonSerializer.Serialize(current, new JsonSerializerOptions { WriteIndented = false });
                }
                catch
                {
                    dadosNovos = "{}";
                }
            }

            // IP / UserAgent
            var ip = _http.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "";
            var userAgent = _http.HttpContext?.Request?.Headers["User-Agent"].ToString() ?? "";

            // Usuario / Tenant via CurrentUserService (pode ser null -> 0)
            var idUsuario = _currentUser?.UserId ?? 0;
            var idTenant = _currentUser?.TenantId ?? 0;

            //var idUsuario = 1;
            //var idTenant = 1;

            // Converter idRegistro para long na medida do possível (se for composto, 0)
            long idRegistro = 0;
            if (pkValues.Length == 1)
            {
                if (!long.TryParse(idRegistroStr, out idRegistro))
                    idRegistro = 0;

                if (idRegistro < 0) idRegistro = 0;
            }


            var log = new AuditLog
            {
                IdTenant = idTenant,
                IdUsuario = idUsuario,
                Tabela = entityName,
                IdRegistro = idRegistro,
                Acao = acao.ToString(),
                DadosAntigos = string.IsNullOrWhiteSpace(dadosAntigos) ? "{}" : dadosAntigos,
                DadosNovos = string.IsNullOrWhiteSpace(dadosNovos) ? "{}" : dadosNovos,
                IpAddress = ip,
                UserAgent = userAgent,
                CreatedAt = DateTime.UtcNow,
                CreatedYear = DateTime.UtcNow.Year
            };

            return log;
        }
    }
}

