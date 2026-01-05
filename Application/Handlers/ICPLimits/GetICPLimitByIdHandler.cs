using Application.Exceptions.ICPLimits;
using Application.Queries.ICPLimits;
using AutoMapper;
using Domain.Contracts.ICPLimits;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.ICPLimits
{
    public class GetICPLimitByIdHandler : IRequestHandler<GetICPLimitByIdQuery, GetApiResponse>
    {
        private readonly IBaseRepository<ICPLimit> _baseRepository;
        private readonly IMapper _mapper;
        public GetICPLimitByIdHandler(IBaseRepository<ICPLimit> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<GetApiResponse> Handle(GetICPLimitByIdQuery request, CancellationToken cancellationToken)
        {
            var icp = await _baseRepository.GetByIdAsync(request.Id);

            if (icp == null)
            {
                throw new ICPLimitNotFoundException(request.Id);
            }

            var result = _mapper.Map<ICPLimitDto>(icp);

            return new GetApiResponse
            {
                Data = result,
                Message = "ICP encontrado com sucesso"
            };
        }
    }
}
