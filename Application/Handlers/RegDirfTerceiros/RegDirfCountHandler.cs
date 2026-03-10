using Application.Queries.RegDirfTerceiros;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegDirfTerceiros
{
    public class RegDirfCountHandler : IRequestHandler<RegDirfCountQuery, GetApiResponse>
    {
        private readonly IBaseRepository<RegDirfTerceiro> _regDirfTerceirosRepository;

        public RegDirfCountHandler(IBaseRepository<RegDirfTerceiro> regDirfTerceirosRepository)
        {
            _regDirfTerceirosRepository = regDirfTerceirosRepository;
        }

        public async Task<GetApiResponse> Handle(RegDirfCountQuery request, CancellationToken cancellationToken)
        {
            var count = await _regDirfTerceirosRepository.CountAsync(x => x.IdFilename == request.IdFilename);

            return new GetApiResponse
            {
                Message = "",
                Data = count
            };
        }
    }
}
