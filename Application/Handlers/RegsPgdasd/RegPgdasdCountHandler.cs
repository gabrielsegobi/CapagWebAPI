using Application.Queries.RegsPgdasd;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

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
            var count = await _regPgdasdRepository.CountAsync(x => x.IdFilename == request.IdFilename);

            return new GetApiResponse
            {
                Message = "",
                Data = count
            };
        }
    }
}
