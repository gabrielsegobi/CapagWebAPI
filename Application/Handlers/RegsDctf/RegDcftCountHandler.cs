using Application.Exceptions.RegsDctf;
using Application.Queries.RegsDctf;
using Domain.Contracts.RegsDctf;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.RegsDctf
{
    public class RegDcftCountHandler : IRequestHandler<RegDcftCountQuery, GetApiResponse>
    {
        private readonly IBaseRepository<RegDctf> _regDctfRepository;

        public RegDcftCountHandler(IBaseRepository<RegDctf> regDctfRepository)
        {
            _regDctfRepository = regDctfRepository;
        }

        public async Task<GetApiResponse> Handle(RegDcftCountQuery request, CancellationToken cancellationToken)
        {
            var result = await _regDctfRepository.Query()
                     .Where(x => x.IdFilename == request.IdFilename)
                     .GroupBy(x => 1)
                     .Select(g => new RegDctfCountDto
                     {
                         TotalCount = g.Count(),
                         Valor = g.Sum(x => x.Valor),
                     })
                     .FirstOrDefaultAsync(cancellationToken)
                     ?? throw new RegDctfFileNotFoundException(request.IdFilename);

            return new GetApiResponse
            {
                Message = "",
                Data = result
            };
        }
    }
}
