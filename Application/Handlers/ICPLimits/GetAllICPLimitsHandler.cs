using Application.Queries.ICPLimits;
using AutoMapper;
using Domain.Contracts.ICPLimits;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.ICPLimits
{
    public class GetAllICPLimitsHandler : IRequestHandler<GetAllICPLimitsQuery, PagedApiResponse<ICPLimitDto>>
    {
        private readonly IBaseRepository<ICPLimit> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllICPLimitsHandler(IBaseRepository<ICPLimit> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<PagedApiResponse<ICPLimitDto>> Handle(GetAllICPLimitsQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<ICPLimit, ICPLimitDto>(
                request.Filter,
                applyFilters: q =>
                {

                    if (!string.IsNullOrWhiteSpace(request.Filter.ColorCode))
                        q = q.Where(e => e.ColorCode.ToString().Contains(request.Filter.ColorCode.Trim()));

                   if (request.Filter.IsActive.HasValue)
                       q = q.Where(e => e.Active == request.Filter.IsActive.Value);
  
                    q = request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.SortOrder)
                        : q.OrderBy(e => e.SortOrder);

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<ICPLimitDto>>(data)
            );

            return pagedResult;
        }
    }
}
