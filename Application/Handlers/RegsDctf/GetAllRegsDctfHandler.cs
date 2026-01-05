using Application.Queries.RegsDctf;
using AutoMapper;
using Domain.Contracts.RegsDctf;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegsDctf
{
    public class GetAllRegsDctfHandler : IRequestHandler<GetAllRegsDctfQuery, PagedApiResponse<RegDctfDto>>
    {
        private readonly IBaseRepository<RegDctf> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllRegsDctfHandler(IBaseRepository<RegDctf> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<PagedApiResponse<RegDctfDto>> Handle(GetAllRegsDctfQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<RegDctf, RegDctfDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (request.Filter.IdEmpresa >= 0)
                        q = q.Where(e => e.IdEmpresa == request.Filter.IdEmpresa);

                    q = request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.IdEmpresa)
                        : q.OrderBy(e => e.IdEmpresa);

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<RegDctfDto>>(data)
            );

            return pagedResult;
        }
    }
}
