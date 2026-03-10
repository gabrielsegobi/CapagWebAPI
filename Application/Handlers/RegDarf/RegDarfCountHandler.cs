using Application.Queries.RegDarf;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

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
            var count = await _regDarfRepository.CountAsync(x => x.IdFilename == request.IdFilename);

            return new GetApiResponse
            {
                Message = "",
                Data = count
            };
        }
    }
}
