using Application.Exceptions.Empresas;
using Application.Queries.CapagSimples;
using Domain.Contracts.CapagSimples;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.CapagSimples
{
    public class GetSimplesDeclarationHandler
        : IRequestHandler<GetSimplesDeclarationQuery, SimplesDeclarationResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<SimplesDeclaration> _declarationRepository;
        private readonly ICurrentUserService _currentUser;

        public GetSimplesDeclarationHandler(
            IBaseRepository<Empresa> empresaRepository,
            IBaseRepository<SimplesDeclaration> declarationRepository,
            ICurrentUserService currentUser)
        {
            _empresaRepository = empresaRepository;
            _declarationRepository = declarationRepository;
            _currentUser = currentUser;
        }

        public async Task<SimplesDeclarationResponse> Handle(
            GetSimplesDeclarationQuery request,
            CancellationToken cancellationToken)
        {
            await EnsureEmpresaAsync(request.IdEmpresa);

            var declaration = await _declarationRepository
                .Query(d => d.IdEmpresa == request.IdEmpresa, asNoTracking: true)
                .Include(d => d.ExerciseYears)
                .FirstOrDefaultAsync(cancellationToken);

            if (declaration == null)
                return SimplesDeclarationResponse.Empty();

            return new SimplesDeclarationResponse
            {
                HasDeclaration = true,
                DeclarationKind = declaration.DeclarationKind,
                ExerciseYears = declaration.ExerciseYears
                    .Select(y => y.ExerciseYear)
                    .Distinct()
                    .OrderBy(y => y)
                    .ToList(),
                CreatedAt = declaration.DateCreate,
                UpdatedAt = declaration.DateUpdate
            };
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
