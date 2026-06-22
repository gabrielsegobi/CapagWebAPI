using Application.Queries.DemonstrativosContabeis;
using Domain.Contracts.DemonstrativosContabeis;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.DemonstrativosContabeis
{
    public class GetContasContabeisHandler : IRequestHandler<GetContasContabeisQuery, List<ContaContabilDto>>
    {
        private readonly IBaseRepository<DemonstrativoContabil> _demonstrativoRepository;
        private readonly ICurrentUserService _currentUser;

        public GetContasContabeisHandler(
            IBaseRepository<DemonstrativoContabil> demonstrativoRepository,
            ICurrentUserService currentUser)
        {
            _demonstrativoRepository = demonstrativoRepository;
            _currentUser = currentUser;
        }

        public async Task<List<ContaContabilDto>> Handle(GetContasContabeisQuery request, CancellationToken cancellationToken)
        {
            var tenantHeader = _currentUser.TenantId;
            if (!tenantHeader.HasValue)
                throw new ArgumentException("Tenant não informado.");

            if (request.IdTenant != tenantHeader.Value)
                throw new ArgumentException("id_tenant não confere com o tenant do header.");

            return await _demonstrativoRepository.Query(dc =>
                    dc.IdTenant == request.IdTenant &&
                    dc.DeletedAt == null &&
                    !string.IsNullOrWhiteSpace(dc.Codigo) &&
                    (!request.IdEmpresa.HasValue || dc.IdEmpresa == request.IdEmpresa.Value))
                .GroupBy(dc => new { dc.Codigo, dc.Descricao })
                .Select(g => new ContaContabilDto
                {
                    Codigo = g.Key.Codigo,
                    Descricao = g.Key.Descricao ?? string.Empty
                })
                .OrderBy(x => x.Codigo)
                .ThenBy(x => x.Descricao)
                .ToListAsync(cancellationToken);
        }
    }
}
