using Application.Exceptions.RegDarf;
using Application.Queries.RegDarf;
using Domain.Contracts.RegDarf;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.RegDarf
{
    public class RegDarfCountHandler : IRequestHandler<RegDarfCountQuery, GetApiResponse>
    {
        private readonly IBaseRepository<RegDarfs> _regDarfRepository;

        public RegDarfCountHandler(IBaseRepository<RegDarfs> regDarfRepository)
        {
            _regDarfRepository = regDarfRepository;
        }

        public async Task<GetApiResponse> Handle(RegDarfCountQuery request, CancellationToken cancellationToken)
        {
            var result = await _regDarfRepository.Query()
                .Where(x => x.IdFilename == request.IdFilename)
                .GroupBy(x => 1)
                .Select(g => new RegDarfCountDto
                {
                    TotalCount = g.Count(),
                    ValorTotal = g.Sum(x => x.ValorTotal)
                })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new RegDarfFileNotFoundException(request.IdFilename);


            return new GetApiResponse
            {
                Message = "",
                Data = result
            };
        }
    }
}
