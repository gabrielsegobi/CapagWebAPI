using Application.Queries.DescricaoDebitos;
using AutoMapper;
using Domain.Contracts.DescricaoDebitos;
using Domain.Contracts.Responses;
using Domain.Contracts.ValorCalcVariaveis;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.DescricaoDebitos
{
    public class GetAllDescricoesDebitosHandler : IRequestHandler<GetAllDescricoesDebitosQuery, PagedApiResponse<DescricaoDebitoDto>>
    {
        private readonly IBaseRepository<DescricaoDebito> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllDescricoesDebitosHandler(IBaseRepository<DescricaoDebito> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<DescricaoDebitoDto>> Handle(GetAllDescricoesDebitosQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<DescricaoDebito, DescricaoDebitoDto>(
                request.Filter,
                applyFilters: q =>
                {

                    if (request.Filter.IdEmpresa.HasValue)
                        q = q.Where(e => e.IdEmpresa == request.Filter.IdEmpresa);

                    if (!string.IsNullOrWhiteSpace(request.Filter.Natureza))
                        q = q.Where(e => e.Natureza.Contains(request.Filter.Natureza.Trim()));

                    if (!string.IsNullOrWhiteSpace(request.Filter.NumCda))
                        q = q.Where(e => e.NumCda.Contains(request.Filter.NumCda.Trim()));

                    //if (request.Filter.DataInscricao.HasValue)
                    //    q = q.Where(e => e.DataInscricao >= request.Filter.DataInscricao);

                    if (request.Filter.ValorPrincipal.HasValue)
                        q = q.Where(e => e.ValorPrincipal >= request.Filter.ValorPrincipal);

                    if (request.Filter.ValorMulta.HasValue)
                        q = q.Where(e => e.ValorMulta >= request.Filter.ValorMulta);

                    if (request.Filter.ValorJuros.HasValue)
                        q = q.Where(e => e.ValorJuros >= request.Filter.ValorJuros);

                    if (request.Filter.ValorEncargos.HasValue)
                        q = q.Where(e => e.ValorEncargos >= request.Filter.ValorEncargos);

                    var sort = request.Filter.Sort?.Trim().ToLower();

                    q = sort switch
                    {
                        "idempresa" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.IdEmpresa)
                            : q.OrderBy(e => e.IdEmpresa),

                        "natureza" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.Natureza)
                            : q.OrderBy(e => e.Natureza),

                        "numcda" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.NumCda)
                            : q.OrderBy(e => e.NumCda),

                        "datainscricao" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.DataInscricao)
                            : q.OrderBy(e => e.DataInscricao),

                        "valorprincipal" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.ValorPrincipal)
                            : q.OrderBy(e => e.ValorPrincipal),

                        "valormulta" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.ValorMulta)
                            : q.OrderBy(e => e.ValorMulta),

                        "valorjuros" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.ValorJuros)
                            : q.OrderBy(e => e.ValorJuros),

                        "valorencargos" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.ValorEncargos)
                            : q.OrderBy(e => e.ValorEncargos),

                        _ => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.IdDescricaoDebitos)
                            : q.OrderBy(e => e.IdDescricaoDebitos)
                    };
                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<DescricaoDebitoDto>>(data)
            );

            return pagedResult;
        }
    }
}
