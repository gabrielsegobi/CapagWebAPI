using Application.Queries.RegsDctf;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

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
            var count = await _regDctfRepository.CountAsync(x => x.IdFilename == request.IdFilename);

            return new GetApiResponse
            {
                Message = "",
                Data = count
            };
        }
    }
}
