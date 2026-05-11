using Application.Queries.DemonstrativosContabeis;
using Domain.Contracts.DemonstrativosContabeis;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.DemonstrativosContabeis
{
    public class GetAnosDisponiveisHandler : IRequestHandler<GetAnosDisponiveisQuery, AnosDisponiveisResponse>
    {
        private readonly IBaseRepository<RegDefi> _defisRepository;
        private readonly IBaseRepository<RegPgdasd> _pgdasdRepository;
        private readonly IBaseRepository<DemonstrativoContabil> _demonstrativoRepository;
        private readonly ICurrentUserService _currentUser;

        public GetAnosDisponiveisHandler(
            IBaseRepository<RegDefi> defisRepository,
            IBaseRepository<RegPgdasd> pgdasdRepository,
            IBaseRepository<DemonstrativoContabil> demonstrativoRepository,
            ICurrentUserService currentUser)
        {
            _defisRepository = defisRepository;
            _pgdasdRepository = pgdasdRepository;
            _demonstrativoRepository = demonstrativoRepository;
            _currentUser = currentUser;
        }

        public async Task<AnosDisponiveisResponse> Handle(GetAnosDisponiveisQuery request, CancellationToken cancellationToken)
        {
            if (request.IdEmpresa <= 0)
                throw new ArgumentException("id_empresa inválido.");

            var tenantHeader = _currentUser.TenantId;
            if (!tenantHeader.HasValue)
                throw new ArgumentException("Tenant não informado.");

            if (request.IdTenant != tenantHeader.Value)
                throw new ArgumentException("id_tenant não confere com o tenant do header.");

            // DEFIS: ano -> count de registros
            var defisByAno = await _defisRepository.Query(d =>
                    d.IdEmpresa == request.IdEmpresa)
                .GroupBy(d => d.Periodo.Year)
                .Select(g => new { Ano = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            // PGDASD: em memória por causa do Periodo ser string
            var pgdasdRows = await _pgdasdRepository.Query(p =>
                    p.IdEmpresa == request.IdEmpresa)
                .ToListAsync(cancellationToken);

            var pgdasdCompetenciasByAno = pgdasdRows
                .Select(p => TryParsePeriodoPgdasd(p.Periodo))
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .GroupBy(d => d.Year)
                .Select(g => new
                {
                    Ano = g.Key,
                    Competencias = g.Select(d => (d.Year, d.Month)).Distinct().Count()
                })
                .ToList();

            // Demonstrativos já existentes (não deletados)
            var demonstrativosByAno = await _demonstrativoRepository.Query(dc =>
                    dc.IdEmpresa == request.IdEmpresa &&
                    dc.DeletedAt == null)
                .GroupBy(dc => dc.Ano)
                .Select(g => new { Ano = g.Key, Existe = g.Any() })
                .ToListAsync(cancellationToken);

            var anos = defisByAno.Select(x => x.Ano)
                .Union(pgdasdCompetenciasByAno.Select(x => x.Ano))
                .Union(demonstrativosByAno.Select(x => x.Ano))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var defisLookup = defisByAno.ToDictionary(x => x.Ano, x => x.Count);
            var pgdasdLookup = pgdasdCompetenciasByAno.ToDictionary(x => x.Ano, x => x.Competencias);
            var demoLookup = demonstrativosByAno.ToDictionary(x => x.Ano, x => x.Existe);

            var response = new AnosDisponiveisResponse
            {
                Anos = anos.Select(ano => new AnoDisponivelItem
                {
                    Ano = ano,
                    RegistrosDefis = defisLookup.TryGetValue(ano, out var cDefis) ? cDefis : 0,
                    CompetenciasPgdasd = pgdasdLookup.TryGetValue(ano, out var cPg) ? cPg : 0,
                    JaExisteDemonstrativo = demoLookup.TryGetValue(ano, out var existe) && existe
                }).ToList()
            };

            response.Total = response.Anos.Count;
            return response;
        }

        private static DateTime? TryParsePeriodoPgdasd(string? periodo)
        {
            if (string.IsNullOrWhiteSpace(periodo))
                return null;

            // Aceita "YYYY-MM" ou "YYYY-MM-DD"
            var s = periodo.Trim();
            if (s.Length == 7)
                s += "-01";

            if (DateTime.TryParse(s, out var dt))
                return dt;

            return null;
        }
    }
}

