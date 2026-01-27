using Application.Queries.ValorCalcVariaveis;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Contracts.Usuarios;
using Domain.Contracts.ValorCalcVariaveis;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.ValorCalcVariaveis
{
    public class GetAllValorCalcVariavelHandler : IRequestHandler<GetAllValorCalcVariavelQuery, PagedApiResponse<ValorCalcVariavelDto>>
    {
        private readonly IBaseRepository<ValorCalcVariavel> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllValorCalcVariavelHandler(IBaseRepository<ValorCalcVariavel> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<ValorCalcVariavelDto>> Handle(GetAllValorCalcVariavelQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<ValorCalcVariavel, ValorCalcVariavelDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (request.Filter.IdEmpresa >= 0)
                        q = q.Where(e => e.IdEmpresa == request.Filter.IdEmpresa);

                    if (request.Filter.IdTipoGrupo >= 0)
                        q = q.Where(e => e.IdTipoGrupo == request.Filter.IdTipoGrupo);

                    if (request.Filter.Valor >= 0)
                        q = q.Where(e => e.Valor >= request.Filter.Valor);

                    if (request.Filter.AnoBase >= 0)
                        q = q.Where(e => e.AnoBase == request.Filter.AnoBase);

                    if (!string.IsNullOrWhiteSpace(request.Filter.IdVariavel))
                        q = q.Where(e => e.IdVariavel.ToString().Contains(request.Filter.IdVariavel.Trim()));

                    if (!string.IsNullOrWhiteSpace(request.Filter.Status))
                        q = q.Where(e => e.Status.ToString().Contains(request.Filter.Status.Trim()));

                    var sort = request.Filter.Sort?.Trim().ToLower();

                    q = sort switch
                    {
                        "IdEmpresa" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.IdEmpresa)
                            : q.OrderBy(e => e.IdEmpresa),

                        "IdTipoGrupo" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.IdTipoGrupo)
                            : q.OrderBy(e => e.IdTipoGrupo),

                        "AnoBase" => request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.AnoBase)
                        : q.OrderBy(e => e.AnoBase),

                        "IdVariavel" => request.Filter.OrderByDescending
                       ? q.OrderByDescending(e => e.IdVariavel)
                       : q.OrderBy(e => e.IdVariavel),


                        "Valor" => request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.Valor)
                        : q.OrderBy(e => e.Valor),

                        "Status" => request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.Status)
                        : q.OrderBy(e => e.Status),


                        _ => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.IdValorCalcVariavel)
                            : q.OrderBy(e => e.IdValorCalcVariavel)
                    };

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<ValorCalcVariavelDto>>(data)
            );

            return pagedResult;
        }
    }
}
