using Application.Queries.RegDefis;
using AutoMapper;
using Domain.Contracts.RegDefis;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegDefis
{
    public class GetAllRegDefisHandler : IRequestHandler<GetAllRegDefisQuery, PagedApiResponse<RegDefisDto>>
    {
        private readonly IBaseRepository<RegDefi> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllRegDefisHandler(IBaseRepository<RegDefi> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<RegDefisDto>> Handle(GetAllRegDefisQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<RegDefi, RegDefisDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (request.Filter.IdEmpresa >= 0)
                        q = q.Where(e => e.IdEmpresa == request.Filter.IdEmpresa);

                    if (request.Filter.IdFilename >= 0)
                        q = q.Where(e => e.IdFilename == request.Filter.IdFilename);

                    if (request.Filter.Valor >= 0)
                        q = q.Where(e => e.Valor >= request.Filter.Valor);

                    if (!string.IsNullOrWhiteSpace(request.Filter.Descricao))
                        q = q.Where(e => e.Descricao.ToString().Contains(request.Filter.Descricao.Trim()));

                    if (request.Filter.Periodo.HasValue)
                    {
                        var inicio = new DateTime(
                            request.Filter.Periodo.Value.Year,
                            request.Filter.Periodo.Value.Month,
                            1);

                        var fim = inicio.AddMonths(1);

                        q = q.Where(e => e.Periodo >= inicio && e.Periodo < fim);
                    }


                    var sort = request.Filter.Sort?.Trim().ToLower();

                    q = sort switch
                    {
                        "descricao" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.Descricao)
                            : q.OrderBy(e => e.Descricao),

                        "valor" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.Valor)
                            : q.OrderBy(e => e.Valor),

                        "periodo" => request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.Periodo)
                        : q.OrderBy(e => e.Periodo),

                        _ => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.Id)
                            : q.OrderBy(e => e.Id)
                    };

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<RegDefisDto>>(data)
            );

            return pagedResult;
        }
    }
}
