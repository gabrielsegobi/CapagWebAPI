using Application.Queries.Carteira;
using Domain.Contracts.Carteira;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.Carteira
{
    public class GetCarteiraEmpresasHandler : IRequestHandler<GetCarteiraEmpresasQuery, PagedApiResponse<CarteiraEmpresaDto>>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<CapagCalculadoraResultado> _capagRepository;
        private readonly IBaseRepository<DemonstrativoContabil> _demonstrativoRepository;
        private readonly IBaseRepository<Usuario> _usuarioRepository;

        public GetCarteiraEmpresasHandler(
            IBaseRepository<Empresa> empresaRepository,
            IBaseRepository<CapagCalculadoraResultado> capagRepository,
            IBaseRepository<DemonstrativoContabil> demonstrativoRepository,
            IBaseRepository<Usuario> usuarioRepository)
        {
            _empresaRepository = empresaRepository;
            _capagRepository = capagRepository;
            _demonstrativoRepository = demonstrativoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<PagedApiResponse<CarteiraEmpresaDto>> Handle(
            GetCarteiraEmpresasQuery request,
            CancellationToken cancellationToken)
        {
            var filter = request.Filter;
            int page = filter.Page ?? 1;
            int pageSize = filter.PageSize ?? 10;

            var ratings = await _capagRepository
                .Query(r => !r.Parcial)
                .Select(r => new { r.IdEmpresa, r.Classificacao, r.DateUpdate })
                .ToListAsync(cancellationToken);

            var ratingPorEmpresa = ratings
                .GroupBy(r => r.IdEmpresa)
                .Select(g =>
                {
                    var latest = g.OrderByDescending(r => r.DateUpdate).First();
                    return new
                    {
                        IdEmpresa = g.Key,
                        latest.Classificacao,
                        DataCalculo = latest.DateUpdate
                    };
                })
                .ToList();

            var anoPorEmpresa = await _demonstrativoRepository
                .Query(d => d.DeletedAt == null)
                .GroupBy(d => d.IdEmpresa)
                .Select(g => new { IdEmpresa = g.Key, UltimoAno = g.Max(d => d.Ano) })
                .ToListAsync(cancellationToken);

            var usuarios = await _usuarioRepository
                .Query()
                .Select(u => new { u.IdUsuario, u.Nome })
                .ToListAsync(cancellationToken);

            var ratingLookup = ratingPorEmpresa.ToDictionary(x => x.IdEmpresa);
            var anoLookup = anoPorEmpresa.ToDictionary(x => x.IdEmpresa, x => x.UltimoAno);
            var usuarioLookup = usuarios.ToDictionary(u => u.IdUsuario, u => u.Nome);

            var query = _empresaRepository.Query(e => e.DeletedAt == null);

            if (!string.IsNullOrWhiteSpace(filter.NomeEmpresa))
                query = query.Where(e => e.RazaoSocial.Contains(filter.NomeEmpresa.Trim()));

            if (!string.IsNullOrWhiteSpace(filter.Cnpj))
                query = query.Where(e => e.Cnpj.Contains(filter.Cnpj.Trim()));

            if (!string.IsNullOrWhiteSpace(filter.Status))
                query = query.Where(e => e.Status == filter.Status.Trim());

            if (filter.DataImpedimento.HasValue)
            {
                var inicio = filter.DataImpedimento.Value.Date;
                var fim = inicio.AddDays(1);
                query = query.Where(e => e.DataImpedimento != null && e.DataImpedimento >= inicio && e.DataImpedimento < fim);
            }

            if (filter.DataImpedimentoAte.HasValue)
            {
                var ate = filter.DataImpedimentoAte.Value.Date.AddDays(1);
                query = query.Where(e => e.DataImpedimento != null && e.DataImpedimento < ate);
            }

            if (!string.IsNullOrWhiteSpace(filter.StatusBloqueio))
            {
                var statusBloqueio = filter.StatusBloqueio.Trim().ToLowerInvariant();
                if (statusBloqueio == "bloqueado")
                    query = query.Where(e => e.DataImpedimento != null);
                else if (statusBloqueio == "liberado")
                    query = query.Where(e => e.DataImpedimento == null);
            }

            if (!string.IsNullOrWhiteSpace(filter.RatingCapag))
            {
                var ratingFiltro = filter.RatingCapag.Trim().ToUpperInvariant();
                var idsRating = ratingPorEmpresa
                    .Where(r => (r.Classificacao ?? string.Empty).Trim().ToUpperInvariant() == ratingFiltro)
                    .Select(r => r.IdEmpresa)
                    .ToList();

                if (idsRating.Count == 0)
                    return EmptyPage(page, pageSize);

                query = query.Where(e => idsRating.Contains(e.IdEmpresa));
            }

            if (filter.UltimoAnoEcf.HasValue)
            {
                var idsAno = anoPorEmpresa
                    .Where(a => a.UltimoAno == filter.UltimoAnoEcf.Value)
                    .Select(a => a.IdEmpresa)
                    .ToList();

                if (idsAno.Count == 0)
                    return EmptyPage(page, pageSize);

                query = query.Where(e => idsAno.Contains(e.IdEmpresa));
            }

            if (filter.DataCalculo.HasValue)
            {
                var dia = filter.DataCalculo.Value.Date;
                var idsData = ratingPorEmpresa
                    .Where(r => r.DataCalculo.Date == dia)
                    .Select(r => r.IdEmpresa)
                    .ToList();

                if (idsData.Count == 0)
                    return EmptyPage(page, pageSize);

                query = query.Where(e => idsData.Contains(e.IdEmpresa));
            }

            var total = await query.CountAsync(cancellationToken);

            var empresas = await query
                .OrderBy(e => e.RazaoSocial)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var lista = empresas.Select(e =>
            {
                ratingLookup.TryGetValue(e.IdEmpresa, out var rating);
                anoLookup.TryGetValue(e.IdEmpresa, out var ano);
                string? nomeResponsavel = e.IdUsuarioResponsavel.HasValue && usuarioLookup.TryGetValue(e.IdUsuarioResponsavel.Value, out var nome)
                    ? nome : null;

                return new CarteiraEmpresaDto
                {
                    IdEmpresa = e.IdEmpresa,
                    NomeEmpresa = e.RazaoSocial,
                    Cnpj = e.Cnpj,
                    Status = e.Status,
                    ValorContrato = e.ValorContrato,
                    DataImpedimento = e.DataImpedimento,
                    StatusBloqueio = e.DataImpedimento.HasValue ? "bloqueado" : "liberado",
                    RatingCapag = rating?.Classificacao?.Trim(),
                    UltimoAnoEcf = ano == 0 ? null : ano,
                    DataCalculo = rating?.DataCalculo,
                    Responsavel = nomeResponsavel
                };
            }).ToList();

            return new PagedApiResponse<CarteiraEmpresaDto>(
                new PageData { Page = page, PageSize = pageSize, Total = total },
                lista);
        }

        private static PagedApiResponse<CarteiraEmpresaDto> EmptyPage(int page, int pageSize) =>
            new(new PageData { Page = page, PageSize = pageSize, Total = 0 }, []);
    }
}
