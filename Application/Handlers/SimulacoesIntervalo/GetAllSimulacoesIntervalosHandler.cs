using Application.Queries.SimulacoesCalc;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Contracts.SimulacoesCalc;
using Domain.Contracts.SimulacoesIntervalo;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.SimulacoesIntervalo
{
    public class GetAllSimulacoesIntervalosHandler : IRequestHandler<GetAllSimulacoesIntervalosQuery, PagedApiResponse<SimulacaoIntervaloDto>>
    {
        private readonly IBaseRepository<SimulacaoIntervalo> _baseRepository;
        private readonly IMapper _mapper;
        public async Task<PagedApiResponse<SimulacaoIntervaloDto>> Handle(GetAllSimulacoesIntervalosQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<SimulacaoIntervalo, SimulacaoIntervaloDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (request.Filter.IdEmpresa >= 0)
                        q = q.Where(e => e.IdEmpresa == request.Filter.IdEmpresa);

                    if (request.Filter.IdSimulacaoCalc >= 0)
                        q = q.Where(e => e.IdSimulacaoCalc == request.Filter.IdSimulacaoCalc);

                   
                    if (request.Filter.MesFim >= 0)
                        q = q.Where(e => e.MesFim == request.Filter.MesFim);

                    if (request.Filter.MesIni >= 0)
                        q = q.Where(e => e.MesIni == request.Filter.MesIni);


                    if (request.Filter.PctMensal >= 0)
                        q = q.Where(e => e.PctMensal >= request.Filter.PctMensal);

                    var sort = request.Filter.Sort?.Trim().ToLower();

                    q = sort switch
                    {
                        "IdEmpresa" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.IdEmpresa)
                            : q.OrderBy(e => e.IdEmpresa),

                        "MesIni" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.MesIni)
                            : q.OrderBy(e => e.MesIni),

                        "MesFim" => request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.MesFim)
                        : q.OrderBy(e => e.MesFim),

                        "PctMensal" => request.Filter.OrderByDescending
                       ? q.OrderByDescending(e => e.PctMensal)
                       : q.OrderBy(e => e.PctMensal),

                        "CreatedAt" => request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.CreatedAt)
                        : q.OrderBy(e => e.CreatedAt),

                        "UpdatedAt" => request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.UpdatedAt)
                        : q.OrderBy(e => e.UpdatedAt),

                        "TipoIntervalo" => request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.TipoIntervalo)
                        : q.OrderBy(e => e.TipoIntervalo),


                        _ => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.IdSimulacaoIntervalo)
                            : q.OrderBy(e => e.IdSimulacaoIntervalo)
                    };

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<SimulacaoIntervaloDto>>(data)
            );

            return pagedResult;
        }
    }
}
