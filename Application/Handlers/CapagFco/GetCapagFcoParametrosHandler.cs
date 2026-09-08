using Application.Exceptions.Empresas;
using Application.Helpers;
using Application.Queries.CapagFco;
using Domain.Contracts.CapagFco;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.CapagFco
{
    public class GetCapagFcoParametrosHandler
        : IRequestHandler<GetCapagFcoParametrosQuery, CapagFcoParametrosResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<CapagFcoParametroEmpresa> _parametroRepository;
        private readonly ICurrentUserService _currentUser;

        public GetCapagFcoParametrosHandler(
            IBaseRepository<Empresa> empresaRepository,
            IBaseRepository<CapagFcoParametroEmpresa> parametroRepository,
            ICurrentUserService currentUser)
        {
            _empresaRepository = empresaRepository;
            _parametroRepository = parametroRepository;
            _currentUser = currentUser;
        }

        public async Task<CapagFcoParametrosResponse> Handle(
            GetCapagFcoParametrosQuery request,
            CancellationToken cancellationToken)
        {
            await EnsureEmpresaAsync(request.IdEmpresa);

            var entity = await _parametroRepository.GetFirstOrDefaultAsync(
                p => p.IdEmpresa == request.IdEmpresa);

            return CapagFcoParametrosHelper.ToResponse(request.IdEmpresa, entity);
        }

        private async Task EnsureEmpresaAsync(long idEmpresa)
        {
            var tenantId = _currentUser.TenantId
                ?? throw new Exceptions.ValidationException(["Tenant não informado."]);

            var empresa = await _empresaRepository.GetByIdAsync(idEmpresa);
            if (empresa == null || empresa.IdTenant != tenantId)
                throw new EmpresaNotFoundException(idEmpresa);
        }
    }
}
