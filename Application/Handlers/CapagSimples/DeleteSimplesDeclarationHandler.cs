using Application.Commands.CapagSimples;
using Application.Exceptions.Empresas;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.CapagSimples
{
    public class DeleteSimplesDeclarationHandler : IRequestHandler<DeleteSimplesDeclarationCommand, Unit>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<SimplesDeclaration> _declarationRepository;
        private readonly ICurrentUserService _currentUser;

        public DeleteSimplesDeclarationHandler(
            IBaseRepository<Empresa> empresaRepository,
            IBaseRepository<SimplesDeclaration> declarationRepository,
            ICurrentUserService currentUser)
        {
            _empresaRepository = empresaRepository;
            _declarationRepository = declarationRepository;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(
            DeleteSimplesDeclarationCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUser.TenantId
                ?? throw new Exceptions.ValidationException(["Tenant não informado."]);

            var empresa = await _empresaRepository.GetByIdAsync(request.IdEmpresa);
            if (empresa == null || empresa.IdTenant != tenantId)
                throw new EmpresaNotFoundException(request.IdEmpresa);

            var declaration = await _declarationRepository
                .Query(d => d.IdEmpresa == request.IdEmpresa, asNoTracking: false)
                .FirstOrDefaultAsync(cancellationToken);

            if (declaration == null)
                return Unit.Value;

            _declarationRepository.Delete(declaration);
            await _declarationRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
