using Application.Queries.ICPAnterior;
using AutoMapper;
using Domain.Contracts.ExtractionRules;
using Domain.Contracts.ICPAnterior;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.ICPAnterior
{
    public class GetAllICPAnteriorHandler : IRequestHandler<GetAllICPAnteriorQuery, PagedApiResponse<ICPAnteriorDto>>
    {
        private readonly IBaseRepository<ICPsAnterior> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllICPAnteriorHandler(IBaseRepository<ICPsAnterior> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<ICPAnteriorDto>> Handle(GetAllICPAnteriorQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<ICPsAnterior, ICPAnteriorDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (request.Filter.IdEmpresa >= 0)
                        q = q.Where(e => e.IdEmpresa == request.Filter.IdEmpresa);


                    if (request.Filter.ValorICPReceita >= 0)
                        q = q.Where(e => e.ValorICPReceita >= request.Filter.ValorICPReceita);

                    if (request.Filter.Classificacao.HasValue)
                        q = q.Where(e => e.Classificacao == request.Filter.Classificacao.Value);


                    var sort = request.Filter.Sort?.Trim().ToLower();

                    q = sort switch
                    {
                        "Classificacao" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.Classificacao)
                            : q.OrderBy(e => e.Classificacao),

                        "IdEmpresa" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.IdEmpresa)
                            : q.OrderBy(e => e.IdEmpresa),

                        "ValorICPReceita" => request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.ValorICPReceita)
                        : q.OrderBy(e => e.ValorICPReceita),

                        _ => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.IdIcpAnterior)
                            : q.OrderBy(e => e.IdIcpAnterior)
                    };

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<ICPAnteriorDto>>(data)
            );

            return pagedResult;
        }
    }
}
