using Application.Queries.Usuarios;
using Domain.Contracts.Usuarios;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.Usuarios
{

    public class GetUserTenantsHandler
          : IRequestHandler<GetUserTenantsQuery, List<UserTenantResponse>>
    {
        private readonly IBaseRepository<UsuarioTenant> _usuarioTenantRepository;
        private readonly IBaseRepository<Tenant> _tenantRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetUserTenantsHandler(
            IBaseRepository<UsuarioTenant> usuarioTenantRepository,
            IBaseRepository<Tenant> tenantRepository,
            ICurrentUserService currentUserService)
        {
            _usuarioTenantRepository = usuarioTenantRepository;
            _tenantRepository = tenantRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<UserTenantResponse>> Handle(
            GetUserTenantsQuery request,
            CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == null)
                throw new UnauthorizedAccessException("Usuário não autenticado");

            var userId = _currentUserService.UserId.Value;

            var vinculosQuery = _usuarioTenantRepository.Query(ut =>
                ut.IdUsuario == userId &&
                ut.Ativo &&
                ut.DeletedAt == null);

            var query =
                from ut in vinculosQuery
                join t in _tenantRepository.Query(t => t.DeletedAt == null)
                    on ut.IdTenant equals t.IdTenant
                select new UserTenantResponse
                {
                    TenantId = t.IdTenant,
                    Nome = t.Nome
                };

            return await query.ToListAsync(cancellationToken);
        }
    }
}
