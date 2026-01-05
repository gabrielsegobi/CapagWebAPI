using Application.Exceptions.Indicadores;
using Application.Queries.Indicadores;
using AutoMapper;
using Domain.Contracts.Indicadores;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Indicadores
{
    public class GetIndicadorByIdHandler : IRequestHandler<GetIndicadorByIdQuery, GetApiResponse>
    {
        private readonly IBaseRepository<Indicador> _baseRepository;
        private readonly IMapper _mapper;
        public GetIndicadorByIdHandler(IBaseRepository<Indicador> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<GetApiResponse> Handle(GetIndicadorByIdQuery request, CancellationToken cancellationToken)
        {
            var indicador = await _baseRepository.GetByIdAsync(request.Id);

            if (indicador == null)
            {
                throw new IndicadorNotFoundException(request.Id);
            }

            var result = _mapper.Map<IndicadorDto>(indicador);
            return new GetApiResponse
            {
                Data = result,
                Message = "Indicador encontrado com sucesso"
            };
        }
    }
}
