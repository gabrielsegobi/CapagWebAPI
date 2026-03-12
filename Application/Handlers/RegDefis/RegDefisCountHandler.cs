using Application.Exceptions.RegDefis;
using Application.Queries.RegDefis;
using Domain.Contracts.RegDefis;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.RegDefis
{
    public class RegDefisCountHandler : IRequestHandler<RegDefisCountQuery, GetApiResponse>
    {
        private readonly IBaseRepository<RegDefi> _regDefisRepository;

        public RegDefisCountHandler(IBaseRepository<RegDefi> regDefisRepository)
        {
            _regDefisRepository = regDefisRepository;
        }

        public async Task<GetApiResponse> Handle(RegDefisCountQuery request, CancellationToken cancellationToken)
        {
            var result = await _regDefisRepository.Query()
                           .Where(x => x.IdFilename == request.IdFilename)
                           .GroupBy(x => 1)
                           .Select(g => new RegDefisCountDto
                           {
                               TotalCount = g.Count(),
                               Valor = g.Sum(x => x.Valor)
                           })
                           .FirstOrDefaultAsync(cancellationToken)
                           ?? throw new RegDefisFileNotFoundException(request.IdFilename);

            return new GetApiResponse
            {
                Message = "",
                Data = result
            };
        }
    }
}
