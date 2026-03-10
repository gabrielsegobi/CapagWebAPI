using Application.Queries.RegsIrpf;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

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
            var count = await _regIrpfRepository.CountAsync(x => x.IdFilename == request.IdFilename);

            return new GetApiResponse
            {
                Message = "",
                Data = count
            };
        }
    }
}
