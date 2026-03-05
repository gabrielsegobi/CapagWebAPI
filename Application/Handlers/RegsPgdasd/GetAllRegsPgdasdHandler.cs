using Application.Queries.RegsPgdasd;
using AutoMapper;
using Domain.Contracts.RegsPgdasd;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegsPgdasd
{
    public class GetAllRegsPgdasdHandler : IRequestHandler<GetAllRegsPgdasdQuery, PagedApiResponse<RegPgdasdDto>>
    {
        private readonly IBaseRepository<RegPgdasd> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllRegsPgdasdHandler(IBaseRepository<RegPgdasd> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<RegPgdasdDto>> Handle(GetAllRegsPgdasdQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<RegPgdasd, RegPgdasdDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (request.Filter.IdEmpresa >= 0)
                        q = q.Where(e => e.IdEmpresa == request.Filter.IdEmpresa);

                    if (request.Filter.IdFilename >= 0)
                        q = q.Where(e => e.IdFilename == request.Filter.IdFilename);

                    q = request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.IdEmpresa)
                        : q.OrderBy(e => e.IdEmpresa);

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<RegPgdasdDto>>(data)
            );

            return pagedResult;
        }
    }
}
