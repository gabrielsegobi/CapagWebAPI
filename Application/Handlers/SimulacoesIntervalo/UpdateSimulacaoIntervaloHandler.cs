using Application.Commands.SimulacoesIntervalo;
using Application.Exceptions.SimulacoesCalc;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.SimulacoesIntervalo
{
    public class UpdateSimulacaoIntervaloHandler : IRequestHandler<UpdateSimulacaoIntervaloCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<SimulacaoIntervalo> _baseRepository;
        private readonly IMapper _mapper;

        public static string UpdateMessage = "Intervalo atualizada com sucesso";
        public UpdateSimulacaoIntervaloHandler(IBaseRepository<SimulacaoIntervalo> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<UpdateApiResponse> Handle(UpdateSimulacaoIntervaloCommand request, CancellationToken cancellationToken)
        {
            var calc = await _baseRepository.GetByIdAsync(request.Id)
                  ?? throw new SimulacaoCalcNotFoundException(request.Id);

            _mapper.Map(request.Request, calc);

            _baseRepository.Update(calc);
            await _baseRepository.SaveChangesAsync();

            return new UpdateApiResponse { Message = UpdateMessage };
        }
    }
}
