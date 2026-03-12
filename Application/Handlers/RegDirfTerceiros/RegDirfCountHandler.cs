using Application.Exceptions.RegDirfTerceiros;
using Application.Queries.RegDirfTerceiros;
using Domain.Contracts.RegDirfTerceiros;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            var result = await _regDirfTerceirosRepository.Query()
                       .Where(x => x.IdFilename == request.IdFilename)
                       .GroupBy(x => 1)
                       .Select(g => new RegDirfCountDto
                       {
                           TotalCount = g.Count(),
                           ValorRendimento = g.Sum(x => x.ValorRendimento),
                           ValorTributo = g.Sum(x => x.ValorTributo),
                       })
                       .FirstOrDefaultAsync(cancellationToken)
                       ?? throw new RegDirfFileNotFoundException(request.IdFilename);

            return new GetApiResponse
            {
                Message = "",
                Data = result
            };
        }
    }
}
