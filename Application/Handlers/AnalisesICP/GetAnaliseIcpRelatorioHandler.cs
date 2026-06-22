using Application.Queries.AnalisesICP;
using Domain.Contracts.AnalisesICP;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using System.Globalization;

namespace Application.Handlers.AnalisesICP
{
    public class GetAnaliseIcpRelatorioHandler
        : IRequestHandler<GetAnaliseIcpRelatorioQuery, PagedApiResponse<AnaliseIcpRelatorioDto>>
    {
        private readonly IBaseRepository<AnaliseICP> _analiseIcpRepository;
        private readonly IBaseRepository<Empresa> _empresaRepository;

        public GetAnaliseIcpRelatorioHandler(
            IBaseRepository<AnaliseICP> analiseIcpRepository,
            IBaseRepository<Empresa> empresaRepository)
        {
            _analiseIcpRepository = analiseIcpRepository;
            _empresaRepository = empresaRepository;
        }

        public async Task<PagedApiResponse<AnaliseIcpRelatorioDto>> Handle(
            GetAnaliseIcpRelatorioQuery request,
            CancellationToken cancellationToken)
        {
            var query = _analiseIcpRepository.Query(a => a.DeletedAt == null);

            var pagedResult = await query.ReadPage<
                AnaliseICP,
                AnaliseIcpRelatorioDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (request.Filter.IdEmpresa > 0)
                        q = q.Where(x => x.IdEmpresa == request.Filter.IdEmpresa);

                    if (!string.IsNullOrWhiteSpace(request.Filter.Classificacao))
                        q = q.Where(x =>
                            x.Classificacao.Contains(
                                request.Filter.Classificacao.Trim()));

                    if (request.Filter.IcpCalculado.HasValue)
                    {
                        var icp = request.Filter.IcpCalculado.Value;

                        q = q.Where(x => x.IcpCalculado >= icp);
                    }

                    if (request.Filter.SomaPesos.HasValue)
                    {
                        var somaPesos = request.Filter.SomaPesos.Value;

                        q = q.Where(x => x.SomaPesos >= somaPesos);
                    }

                    if (request.Filter.CreatedAt.HasValue)
                    {
                        var createdAt = request.Filter.CreatedAt.Value;

                        q = q.Where(x => x.CreatedAt >= createdAt);
                    }

                    q = request.Filter.OrderByDescending
                        ? q.OrderByDescending(x => x.CreatedAt)
                        : q.OrderBy(x => x.CreatedAt);

                    return q;
                },
                mapFunc: data =>
                {
                    var empresas = _empresaRepository.Query()
                        .Where(e => data.Select(a => a.IdEmpresa).Contains(e.IdEmpresa))
                        .ToDictionary(e => e.IdEmpresa);

                    return data.Select(ai =>
                    {
                        empresas.TryGetValue(ai.IdEmpresa, out var empresa);

                        return new AnaliseIcpRelatorioDto
                        {
                            Cnpj = empresa?.Cnpj ?? string.Empty,
                            RazaoSocial = empresa?.RazaoSocial ?? string.Empty,
                            Classificacao = ai.Classificacao,
                            IcpCalculado = ai.IcpCalculado
                                .ToString("N4", new CultureInfo("pt-BR"))
                        };
                    });
                }
            );

            return pagedResult;
        }
    }
}
