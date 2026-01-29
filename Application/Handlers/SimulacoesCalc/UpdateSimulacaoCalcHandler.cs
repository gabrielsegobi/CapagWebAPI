using Application.Commands.SimulacoesCalc;
using Application.Exceptions.SimulacoesCalc;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.SimulacoesCalc
{
    public class UpdateSimulacaoCalcHandler : IRequestHandler<UpdateSimulacaoCalcCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<SimulacaoCalc> _baseRepository;
        private readonly IMapper _mapper;

        public static string UpdateMessage = "Simulação atualizada com sucesso";
        public UpdateSimulacaoCalcHandler(IBaseRepository<SimulacaoCalc> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<UpdateApiResponse> Handle(UpdateSimulacaoCalcCommand request, CancellationToken cancellationToken)
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
