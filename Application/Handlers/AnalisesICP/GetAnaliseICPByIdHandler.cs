using Application.Exceptions.RegimesTributarios;
using Application.Queries.AnalisesICP;
using AutoMapper;
using Domain.Contracts.AnalisesICP;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.AnalisesICP
{
    public class GetAnaliseICPByIdHandler : IRequestHandler<GetAnaliseICPByIdQuery, GetApiResponse>
    {
        private readonly IBaseRepository<AnaliseICP> _baseRepository;
        private readonly IMapper _mapper;
        public GetAnaliseICPByIdHandler(IBaseRepository<AnaliseICP> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<GetApiResponse> Handle(GetAnaliseICPByIdQuery request, CancellationToken cancellationToken)
        {
            var analise = await _baseRepository.GetByIdAsync(request.Id);

            if (analise == null)
            {
                throw new RegimeTributarioNotFoundException(request.Id);
            }

            var result = _mapper.Map<AnaliseICPDto>(analise);
            return new GetApiResponse
            {
                Data = result,
                Message = " Analise encontrada com sucesso"
            };
        }
    }
}
