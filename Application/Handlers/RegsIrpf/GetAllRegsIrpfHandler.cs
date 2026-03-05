using Application.Queries.RegsIrpf;
using AutoMapper;
using Domain.Contracts.RegIrpf;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegsIrpf
{
    public class GetAllRegsIrpfHandler : IRequestHandler<GetAllRegsIrpfQuery, PagedApiResponse<RegIrpfDto>>
    {
        private readonly IBaseRepository<RegIrpf> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllRegsIrpfHandler(IBaseRepository<RegIrpf> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<RegIrpfDto>> Handle(GetAllRegsIrpfQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<RegIrpf, RegIrpfDto>(
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
                mapFunc: data => _mapper.Map<IEnumerable<RegIrpfDto>>(data)
            );

            return pagedResult;
        }
    }
}
