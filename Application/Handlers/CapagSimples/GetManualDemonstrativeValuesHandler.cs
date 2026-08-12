using Application.Exceptions.Empresas;
using Application.Helpers;
using Application.Queries.CapagSimples;
using Domain.Contracts.CapagSimples;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.CapagSimples
{
    public class GetManualDemonstrativeValuesHandler
        : IRequestHandler<GetManualDemonstrativeValuesQuery, ManualDemonstrativeValuesResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<ManualDemonstrativeValue> _valueRepository;
        private readonly ICurrentUserService _currentUser;

        public GetManualDemonstrativeValuesHandler(
            IBaseRepository<Empresa> empresaRepository,
            IBaseRepository<ManualDemonstrativeValue> valueRepository,
            ICurrentUserService currentUser)
        {
            _empresaRepository = empresaRepository;
            _valueRepository = valueRepository;
            _currentUser = currentUser;
        }

        public async Task<ManualDemonstrativeValuesResponse> Handle(
            GetManualDemonstrativeValuesQuery request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUser.TenantId
                ?? throw new Exceptions.ValidationException(["Tenant não informado."]);

            var empresa = await _empresaRepository.GetByIdAsync(request.IdEmpresa);
            if (empresa == null || empresa.IdTenant != tenantId)
                throw new EmpresaNotFoundException(request.IdEmpresa);

            var kinds = ResolveKinds(request.Kinds);

            var rows = await _valueRepository
                .Query(v => v.IdEmpresa == request.IdEmpresa && kinds.Contains(v.DemonstrativeKind))
                .ToListAsync(cancellationToken);

            var response = new ManualDemonstrativeValuesResponse();

            if (kinds.Contains(CapagSimplesHelper.KindDre))
            {
                response.Dre = CapagSimplesHelper.ToPorCodigoMap(
                    rows.Where(r => r.DemonstrativeKind == CapagSimplesHelper.KindDre)
                        .Select(r => (r.AccountCode, r.ExerciseYear, r.Amount)));
            }

            if (kinds.Contains(CapagSimplesHelper.KindBalanceSheet))
            {
                response.BalanceSheet = CapagSimplesHelper.ToPorCodigoMap(
                    rows.Where(r => r.DemonstrativeKind == CapagSimplesHelper.KindBalanceSheet)
                        .Select(r => (r.AccountCode, r.ExerciseYear, r.Amount)));
            }

            return response;
        }

        private static HashSet<string> ResolveKinds(List<string>? kinds)
        {
            if (kinds == null || kinds.Count == 0)
            {
                return new HashSet<string>(StringComparer.Ordinal)
                {
                    CapagSimplesHelper.KindDre,
                    CapagSimplesHelper.KindBalanceSheet
                };
            }

            var resolved = new HashSet<string>(StringComparer.Ordinal);
            foreach (var kind in kinds)
            {
                if (CapagSimplesHelper.IsValidDemonstrativeKind(kind))
                    resolved.Add(kind);
            }

            if (resolved.Count == 0)
            {
                throw new Exceptions.ValidationException(
                    ["Parâmetro kind inválido. Use DRE e/ou BALANCE_SHEET."]);
            }

            return resolved;
        }
    }
}
