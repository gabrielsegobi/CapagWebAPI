using Application.Exceptions.RegimesTributarios;
using Application.Queries.RegimesTributarios;
using AutoMapper;
using Domain.Contracts.RegimesTributarios;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegimesTributarios
{
    public class GetRegimeTributarioByIdHandler : IRequestHandler<GetRegimeTributarioByIdQuery, GetApiResponse>
    {
        private readonly IBaseRepository<RegimeTributario> _baseRepository;
        private readonly IMapper _mapper;
        public GetRegimeTributarioByIdHandler(IBaseRepository<RegimeTributario> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<GetApiResponse> Handle(GetRegimeTributarioByIdQuery request, CancellationToken cancellationToken)
        {
            var regimetributario = await _baseRepository.GetByIdAsync(request.Id);

            if (regimetributario == null)
            {
                throw new RegimeTributarioNotFoundException(request.Id);
            }

            var result = _mapper.Map<RegimeTributarioDto>(regimetributario);
            return new GetApiResponse
            {
                Data = result,
                Message = "Regime Tributario encontrado com sucesso"
            };
        }
    }
}
