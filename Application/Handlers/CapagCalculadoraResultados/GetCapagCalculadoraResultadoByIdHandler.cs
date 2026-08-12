using Application.Exceptions.CapagCalculadoraResultados;
using Application.Queries.CapagCalculadoraResultados;
using AutoMapper;
using Domain.Contracts.CapagCalculadoraResultados;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.CapagCalculadoraResultados
{
    public class GetCapagCalculadoraResultadoByIdHandler : IRequestHandler<GetCapagCalculadoraResultadoByIdQuery, GetApiResponse>
    {
        private readonly IBaseRepository<CapagCalculadoraResultado> _baseRepository;
        private readonly IMapper _mapper;

        public GetCapagCalculadoraResultadoByIdHandler(IBaseRepository<CapagCalculadoraResultado> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<GetApiResponse> Handle(GetCapagCalculadoraResultadoByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _baseRepository.GetByIdAsync(request.Id)
                ?? throw new CapagCalculadoraResultadoNotFoundException(request.Id);

            return new GetApiResponse
            {
                Data = _mapper.Map<CapagCalculadoraResultadoDto>(entity),
                Message = "Resultado da calculadora CAPAG encontrado com sucesso"
            };
        }
    }
}
