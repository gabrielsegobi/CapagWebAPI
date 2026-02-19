using Application.Queries.Usuarios;
using Domain.Contracts.Usuarios;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Usuarios
{
    public class GetTenantPermissionsHandler : IRequestHandler<GetTenantPermissionsQuery, TenantPermissionResponse>
    {
        private readonly ICurrentUserService _currentUserService;

        public GetTenantPermissionsHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public Task<TenantPermissionResponse> Handle(GetTenantPermissionsQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException("Usuário não autenticado");

            if (_currentUserService.TenantId == null)
                throw new InvalidOperationException("Tenant não definido no contexto");

            if (string.IsNullOrWhiteSpace(_currentUserService.Role))
                throw new InvalidOperationException("Papel do usuário não definido");

            var response = new TenantPermissionResponse
            {
                TenantId = _currentUserService.TenantId.Value,
                Papel = _currentUserService.Role
            };

            return Task.FromResult(response);
        }
    }
}
