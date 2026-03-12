using Application.Exceptions.RegDefis;
using Application.Exceptions.RegIrpf;
using Application.Queries.RegsIrpf;
using Domain.Contracts.RegDefis;
using Domain.Contracts.RegIrpf;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.RegsIrpf
{
    public class RegIrpfCountHandler : IRequestHandler<RegIrpfCountQuery, GetApiResponse>
    {
        private readonly IBaseRepository<RegIrpf> _regIrpfRepository;

        public RegIrpfCountHandler(IBaseRepository<RegIrpf> regIrpfRepository)
        {
            _regIrpfRepository = regIrpfRepository;
        }

        public async Task<GetApiResponse> Handle(RegIrpfCountQuery request, CancellationToken cancellationToken)
        {
            var result = await _regIrpfRepository.Query()
                         .Where(x => x.IdFilename == request.IdFilename)
                         .GroupBy(x => 1)
                         .Select(g => new RegIrpfCountDto
                         {
                             TotalCount = g.Count(),
                             ValorV1 = g.Sum(x => x.ValorV1),
                             ValorV2 = g.Sum(x => x.ValorV2),
                             ValorV3 = g.Sum(x => x.ValorV3),
                             ValorV4 = g.Sum(x => x.ValorV4),
                             ValorV6 = g.Sum(x => x.ValorV6),
                             ValorV7 = g.Sum(x => x.ValorV7)
                         })
                         .FirstOrDefaultAsync(cancellationToken)
                         ?? throw new RegIrpfFileNotFoundException(request.IdFilename);

            return new GetApiResponse
            {
                Message = "",
                Data = result
            };
        }
    }
}
