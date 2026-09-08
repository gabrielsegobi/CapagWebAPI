using Application.Exceptions.CapagE1;
using Application.Exceptions.Empresas;
using Application.Helpers;
using Application.Queries.CapagE1;
using Domain.Contracts.CapagE1;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.CapagE1
{
    public class GetCapagE1CalculoHandler
        : IRequestHandler<GetCapagE1CalculoQuery, CapagE1CalculoPayload>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<CapagE1Calculo> _calculoRepository;
        private readonly ICurrentUserService _currentUser;

        public GetCapagE1CalculoHandler(
            IBaseRepository<Empresa> empresaRepository,
            IBaseRepository<CapagE1Calculo> calculoRepository,
            ICurrentUserService currentUser)
        {
            _empresaRepository = empresaRepository;
            _calculoRepository = calculoRepository;
            _currentUser = currentUser;
        }

        public async Task<CapagE1CalculoPayload> Handle(
            GetCapagE1CalculoQuery request,
            CancellationToken cancellationToken)
        {
            await EnsureEmpresaAsync(request.IdEmpresa);

            var modelo = CapagE1CalculoHelper.NormalizeModelo(request.Modelo);

            var entity = await _calculoRepository.GetFirstOrDefaultAsync(
                c => c.IdEmpresa == request.IdEmpresa && c.Modelo == modelo)
                ?? throw new CapagE1CalculoNotFoundException(request.IdEmpresa, modelo);

            return CapagE1CalculoHelper.ToResponse(entity);
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
