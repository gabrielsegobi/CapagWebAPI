using Application.Queries.RegDefis;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

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
            var count = await _regDefisRepository.CountAsync(x => x.IdFilename == request.IdFilename);

            return new GetApiResponse
            {
                Message = "",
                Data = count
            };

        }
    }
}
