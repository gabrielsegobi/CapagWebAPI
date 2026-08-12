using Application.Commands.CapagSimples;
using Application.Exceptions.Empresas;
using Application.Helpers;
using Domain.Contracts.CapagSimples;
using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.CapagSimples
{
    public class UpsertSimplesDeclarationHandler
        : IRequestHandler<UpsertSimplesDeclarationCommand, SimplesDeclarationResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<SimplesDeclaration> _declarationRepository;
        private readonly IBaseRepository<SimplesExerciseYear> _yearRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly CPGDbContext _db;

        public UpsertSimplesDeclarationHandler(
            IBaseRepository<Empresa> empresaRepository,
            IBaseRepository<SimplesDeclaration> declarationRepository,
            IBaseRepository<SimplesExerciseYear> yearRepository,
            ICurrentUserService currentUser,
            CPGDbContext db)
        {
            _empresaRepository = empresaRepository;
            _declarationRepository = declarationRepository;
            _yearRepository = yearRepository;
            _currentUser = currentUser;
            _db = db;
        }

        public async Task<SimplesDeclarationResponse> Handle(
            UpsertSimplesDeclarationCommand command,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUser.TenantId
                ?? throw new Exceptions.ValidationException(["Tenant não informado."]);

            var empresa = await _empresaRepository.GetByIdAsync(command.IdEmpresa);
            if (empresa == null || empresa.IdTenant != tenantId)
                throw new EmpresaNotFoundException(command.IdEmpresa);

            var kind = command.Request.DeclarationKind;
            var years = kind == CapagSimplesHelper.KindNoNationalSimpleStrict
                ? []
                : CapagSimplesHelper.NormalizeExerciseYears(command.Request.ExerciseYears);

            var now = DateTimeHelper.GetDateTimeNow();
            var idUsuario = _currentUser.UserId;

            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var declaration = await _declarationRepository
                    .Query(d => d.IdEmpresa == command.IdEmpresa, asNoTracking: false)
                    .Include(d => d.ExerciseYears)
                    .FirstOrDefaultAsync(cancellationToken);

                if (declaration == null)
                {
                    declaration = new SimplesDeclaration
                    {
                        IdEmpresa = command.IdEmpresa,
                        IdTenant = tenantId,
                        IdUsuario = idUsuario,
                        DeclarationKind = kind,
                        DateCreate = now,
                        DateUpdate = now
                    };
                    await _declarationRepository.AddAsync(declaration);
                    await _declarationRepository.SaveChangesAsync();
                }
                else
                {
                    declaration.DeclarationKind = kind;
                    declaration.DateUpdate = now;
                    declaration.IdUsuario = idUsuario;
                    _declarationRepository.Update(declaration);

                    if (declaration.ExerciseYears.Count > 0)
                    {
                        _yearRepository.DeleteRange(declaration.ExerciseYears);
                        declaration.ExerciseYears.Clear();
                    }

                    await _declarationRepository.SaveChangesAsync();
                }

                if (years.Count > 0)
                {
                    var yearEntities = years.Select(y => new SimplesExerciseYear
                    {
                        IdSimplesDeclaration = declaration.Id,
                        ExerciseYear = y,
                        DateCreate = now
                    }).ToList();

                    await _yearRepository.AddRangeAsync(yearEntities);
                    await _yearRepository.SaveChangesAsync();
                    declaration.ExerciseYears = yearEntities;
                }

                await tx.CommitAsync(cancellationToken);

                return new SimplesDeclarationResponse
                {
                    HasDeclaration = true,
                    DeclarationKind = declaration.DeclarationKind,
                    ExerciseYears = years,
                    CreatedAt = declaration.DateCreate,
                    UpdatedAt = declaration.DateUpdate
                };
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
