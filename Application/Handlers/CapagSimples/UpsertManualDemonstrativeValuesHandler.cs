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
    public class UpsertManualDemonstrativeValuesHandler
        : IRequestHandler<UpsertManualDemonstrativeValuesCommand, ManualDemonstrativeValuesResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<ManualDemonstrativeValue> _valueRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly CPGDbContext _db;

        public UpsertManualDemonstrativeValuesHandler(
            IBaseRepository<Empresa> empresaRepository,
            IBaseRepository<ManualDemonstrativeValue> valueRepository,
            ICurrentUserService currentUser,
            CPGDbContext db)
        {
            _empresaRepository = empresaRepository;
            _valueRepository = valueRepository;
            _currentUser = currentUser;
            _db = db;
        }

        public async Task<ManualDemonstrativeValuesResponse> Handle(
            UpsertManualDemonstrativeValuesCommand command,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUser.TenantId
                ?? throw new Exceptions.ValidationException(["Tenant não informado."]);

            var empresa = await _empresaRepository.GetByIdAsync(command.IdEmpresa);
            if (empresa == null || empresa.IdTenant != tenantId)
                throw new EmpresaNotFoundException(command.IdEmpresa);

            var kind = command.Request.DemonstrativeKind;
            var rows = CapagSimplesHelper.FlattenPorCodigo(command.Request.PorCodigo);
            var now = DateTimeHelper.GetDateTimeNow();
            var idUsuario = _currentUser.UserId;

            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var existentes = await _valueRepository
                    .Query(
                        v => v.IdEmpresa == command.IdEmpresa && v.DemonstrativeKind == kind,
                        asNoTracking: false)
                    .ToListAsync(cancellationToken);

                if (existentes.Count > 0)
                {
                    _valueRepository.DeleteRange(existentes);
                    await _valueRepository.SaveChangesAsync();
                }

                if (rows.Count > 0)
                {
                    var entities = rows.Select(r => new ManualDemonstrativeValue
                    {
                        IdEmpresa = command.IdEmpresa,
                        IdTenant = tenantId,
                        IdUsuario = idUsuario,
                        DemonstrativeKind = kind,
                        AccountCode = r.AccountCode,
                        ExerciseYear = r.Year,
                        Amount = r.Value,
                        DateCreate = now,
                        DateUpdate = now
                    }).ToList();

                    await _valueRepository.AddRangeAsync(entities);
                    await _valueRepository.SaveChangesAsync();
                }

                await tx.CommitAsync(cancellationToken);
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }

            // Retorna o estado do kind atualizado (e omite o outro).
            var response = new ManualDemonstrativeValuesResponse();
            var map = CapagSimplesHelper.ToPorCodigoMap(
                rows.Select(r => (r.AccountCode, r.Year, r.Value)));

            if (kind == CapagSimplesHelper.KindDre)
                response.Dre = map;
            else
                response.BalanceSheet = map;

            return response;
        }
    }
}
