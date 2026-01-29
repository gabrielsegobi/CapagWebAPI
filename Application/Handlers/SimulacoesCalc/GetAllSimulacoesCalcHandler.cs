using Application.Queries.SimulacoesCalc;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Contracts.SimulacoesCalc;
using Domain.Contracts.ValorCalcVariaveis;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.SimulacoesCalc
{
    public class GetAllSimulacoesCalcHandler : IRequestHandler<GetAllSimulacoesCalcQuery, PagedApiResponse<SimulacaoCalcDto>>
    {
        private readonly IBaseRepository<SimulacaoCalc> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllSimulacoesCalcHandler(IBaseRepository<SimulacaoCalc> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<SimulacaoCalcDto>> Handle(GetAllSimulacoesCalcQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query().Include(sc => sc.SimulacaoIntervalos);

            var pagedResult = await query.ReadPage<SimulacaoCalc, SimulacaoCalcDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (request.Filter.IdEmpresa >= 0)
                        q = q.Where(e => e.IdEmpresa == request.Filter.IdEmpresa);

                    if (request.Filter.HasPrejuizo.HasValue)
                        q = q.Where(e => e.HasPrejuizo == request.Filter.HasPrejuizo.Value);

                    if (request.Filter.HasAbatimento.HasValue)
                        q = q.Where(e => e.HasAbatimento == request.Filter.HasAbatimento.Value);

                    if (request.Filter.LimitadorPCT >= 0)
                        q = q.Where(e => e.LimitadorPCT >= request.Filter.LimitadorPCT);

                    if (request.Filter.DescMaxPct >= 0)
                        q = q.Where(e => e.DescMaxPct >= request.Filter.DescMaxPct);

                    if (request.Filter.PrejuizoValor >= 0)
                        q = q.Where(e => e.PrejuizoValor >= request.Filter.PrejuizoValor);

                    if (request.Filter.AbatimentoValor >= 0)
                        q = q.Where(e => e.AbatimentoValor >= request.Filter.AbatimentoValor);

                    var sort = request.Filter.Sort?.Trim().ToLower();

                    q = sort switch
                    {
                        "IdEmpresa" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.IdEmpresa)
                            : q.OrderBy(e => e.IdEmpresa),

                        "LimitadorPCT" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.LimitadorPCT)
                            : q.OrderBy(e => e.LimitadorPCT),

                        "DescMaxPCT" => request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.DescMaxPct)
                        : q.OrderBy(e => e.DescMaxPct),

                        "TipoSimulacao" => request.Filter.OrderByDescending
                       ? q.OrderByDescending(e => e.TipoSimulacao)
                       : q.OrderBy(e => e.TipoSimulacao),

                        "PrejuizoValor" => request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.PrejuizoValor)
                        : q.OrderBy(e => e.PrejuizoValor),

                        "AbatimentoValor" => request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.AbatimentoValor)
                        : q.OrderBy(e => e.AbatimentoValor),


                        _ => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.IdSimulacaoCalc)
                            : q.OrderBy(e => e.IdSimulacaoCalc)
                    };

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<SimulacaoCalcDto>>(data)
            );

            return pagedResult;
        }
    }
}
