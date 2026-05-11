using Domain.Entities;
using Domain.Entities.Views;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Context
{
    public class CPGDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly AuditInterceptor? _auditInterceptor;
        public CPGDbContext(DbContextOptions<CPGDbContext> options, ICurrentUserService currentUserService, AuditInterceptor? auditInterceptor = null) : base(options)
        {
            _currentUserService = currentUserService;
            _auditInterceptor = auditInterceptor;
        }
        public class TenantAccessException : Exception
        {
            public TenantAccessException(string message) : base(message)
            {
            }
        }
        
        public DbSet<BalancoPatrimonialVw> BalancoPatrimonial { get; set; }
        public DbSet<DashboardEmpresaAnualVw> DashboardEmpresaAnual { get; set; }
        public DbSet<DREVw> DRE { get; set; }
        public DbSet<UsuariosAcessosVw> UsuariosAcessos { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_auditInterceptor != null)
                optionsBuilder.AddInterceptors(_auditInterceptor);

            base.OnConfiguring(optionsBuilder);
        }

        public long? CurrentTenantId => _currentUserService.TenantId;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CPGDbContext).Assembly);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (!typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
                    continue;

                var parameter = Expression.Parameter(entityType.ClrType, "e");

                var efPropertyMethod = typeof(EF).GetMethod(nameof(EF.Property), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)!
                    .MakeGenericMethod(typeof(long?));
                var idTenantProperty = Expression.Call(efPropertyMethod, parameter, Expression.Constant(nameof(ITenantEntity.IdTenant)));

                var currentTenantProperty = Expression.Property(Expression.Constant(this), nameof(CurrentTenantId));

                var body = Expression.Equal(idTenantProperty, currentTenantProperty);

                var lambda = Expression.Lambda(body, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.TenantId;

            foreach (var entry in ChangeTracker.Entries().Where(e => e.Entity is ITenantEntity))
            {
                var entity = (ITenantEntity)entry.Entity;

                switch (entry.State)
                {
                    case EntityState.Added:
                        if (!tenantId.HasValue)
                            throw new TenantAccessException("Tenant não identificado.");

                        if (entity.IdTenant == 0)
                        {
                            entity.IdTenant = tenantId.Value;
                        }
                        else if (entity.IdTenant != tenantId.Value)
                        {
                            throw new TenantAccessException(
                                $"Operação inválida: tentativa de criar registro de outro tenant. (Token: {tenantId.Value}, Enviado: {entity.IdTenant})");
                        }
                        break;

                    case EntityState.Modified:
                    case EntityState.Deleted:
                        if (tenantId.HasValue && entity.IdTenant != tenantId.Value)
                            throw new TenantAccessException(
                                $"Operação inválida: tentativa de modificar registro de outro tenant. (Token: {tenantId.Value}, Registro: {entity.IdTenant})");
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken); 
        }
    }
}
