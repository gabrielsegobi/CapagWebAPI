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

            // Rating mais recente por empresa (parcial = false, maior date_update)
            var ratingPorEmpresa = await _capagRepository
                .Query(r => !r.Parcial)
                .GroupBy(r => r.IdEmpresa)
                .Select(g => new
                {
                    IdEmpresa = g.Key,
                    Classificacao = g.OrderByDescending(r => r.DateUpdate).First().Classificacao,
                    DataCalculo = g.OrderByDescending(r => r.DateUpdate).First().DateUpdate
                })
                .ToListAsync(cancellationToken);

            // Último ano ECF por empresa
            var anoPorEmpresa = await _demonstrativoRepository
                .Query(d => d.DeletedAt == null)
                .GroupBy(d => d.IdEmpresa)
                .Select(g => new { IdEmpresa = g.Key, UltimoAno = g.Max(d => d.Ano) })
                .ToListAsync(cancellationToken);

            // Usuários para lookup de nome do responsável
            var usuarios = await _usuarioRepository
                .Query()
                .Select(u => new { u.IdUsuario, u.Nome })
                .ToListAsync(cancellationToken);

            var ratingLookup = ratingPorEmpresa.ToDictionary(x => x.IdEmpresa);
            var anoLookup = anoPorEmpresa.ToDictionary(x => x.IdEmpresa, x => x.UltimoAno);
            var usuarioLookup = usuarios.ToDictionary(u => u.IdUsuario, u => u.Nome);

            // Projeta query de empresas com filtros
            var query = _empresaRepository.Query();

            if (!string.IsNullOrWhiteSpace(filter.NomeEmpresa))
                query = query.Where(e => e.RazaoSocial.Contains(filter.NomeEmpresa.Trim()));

            if (!string.IsNullOrWhiteSpace(filter.Cnpj))
                query = query.Where(e => e.Cnpj.Contains(filter.Cnpj.Trim()));

            if (!string.IsNullOrWhiteSpace(filter.Status))
                query = query.Where(e => e.Status == filter.Status.Trim());

            if (filter.DataImpedimento.HasValue)
            {
                var ate = filter.DataImpedimento.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(e => e.DataImpedimento != null && e.DataImpedimento <= ate);
            }

            var total = await query.CountAsync(cancellationToken);

            int skip = ((filter.Page ?? 1) - 1) * (filter.PageSize ?? 10);
            int take = filter.PageSize ?? 10;

            var empresas = await query
                .OrderBy(e => e.RazaoSocial)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);

            // Monta DTOs em memória (joins com lookup)
            var dtos = empresas.Select(e =>
            {
                ratingLookup.TryGetValue(e.IdEmpresa, out var rating);
                anoLookup.TryGetValue(e.IdEmpresa, out var ano);
                string? nomeResponsavel = e.IdUsuarioResponsavel.HasValue && usuarioLookup.TryGetValue(e.IdUsuarioResponsavel.Value, out var nome)
                    ? nome : null;

                var statusBloqueio = e.DataImpedimento.HasValue ? "bloqueado" : "liberado";

                return new CarteiraEmpresaDto
                {
                    IdEmpresa = e.IdEmpresa,
                    NomeEmpresa = e.RazaoSocial,
                    Cnpj = e.Cnpj,
                    Status = e.Status,
                    ValorContrato = e.ValorContrato,
                    DataImpedimento = e.DataImpedimento,
                    StatusBloqueio = statusBloqueio,
                    RatingCapag = rating?.Classificacao,
                    UltimoAnoEcf = ano == 0 ? null : ano,
                    DataCalculo = rating?.DataCalculo,
                    Responsavel = nomeResponsavel
                };
            }).AsEnumerable();

            // Filtros em memória (dependem dos dados dos joins)
            if (!string.IsNullOrWhiteSpace(filter.StatusBloqueio))
                dtos = dtos.Where(d => d.StatusBloqueio == filter.StatusBloqueio.Trim().ToLower());

            if (!string.IsNullOrWhiteSpace(filter.RatingCapag))
                dtos = dtos.Where(d => d.RatingCapag == filter.RatingCapag.Trim().ToUpperInvariant());

            if (filter.UltimoAnoEcf.HasValue)
                dtos = dtos.Where(d => d.UltimoAnoEcf == filter.UltimoAnoEcf.Value);

            if (filter.DataCalculo.HasValue)
            {
                var dia = filter.DataCalculo.Value.Date;
                dtos = dtos.Where(d => d.DataCalculo.HasValue && d.DataCalculo.Value.Date == dia);
            }

            var lista = dtos.ToList();

            var pageData = new PageData
            {
                Page = filter.Page ?? 1,
                PageSize = filter.PageSize ?? 10,
                Total = total
            };

            return new PagedApiResponse<CarteiraEmpresaDto>(pageData, lista);
        }
    }
}
