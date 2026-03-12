using Application.Exceptions.RegsPgdasd;
using Application.Queries.RegsPgdasd;
using Domain.Contracts.RegsPgdasd;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.RegsPgdasd
{
    public class RegPgdasdCountHandler : IRequestHandler<RegPgdasdCountQuery, GetApiResponse>
    {
        private readonly IBaseRepository<RegPgdasd> _regPgdasdRepository;

        public RegPgdasdCountHandler(IBaseRepository<RegPgdasd> regPgdasdRepository)
        {
            _regPgdasdRepository = regPgdasdRepository;
        }

        public async Task<GetApiResponse> Handle(RegPgdasdCountQuery request, CancellationToken cancellationToken)
        {
            var result = await _regPgdasdRepository.Query()
                   .Where(x => x.IdFilename == request.IdFilename)
                   .GroupBy(x => 1)
                   .Select(g => new RegPgdasdCountDto
                   {
                       TotalCount = g.Count(),
                       ReceitaBruta = g.Sum(x => x.ReceitaBruta),
                       TotalDebito = g.Sum(x => x.TotalDebito)
                   })
                   .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new RegPgdasdFileNotFoundException(request.IdFilename);


            return new GetApiResponse
            {
                Message = "",
                Data = result
            };
        }
    }
}
