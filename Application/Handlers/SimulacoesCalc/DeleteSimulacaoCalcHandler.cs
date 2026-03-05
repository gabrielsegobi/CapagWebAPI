using Application.Commands.SimulacoesCalc;
using Application.Exceptions.SimulacoesCalc;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.SimulacoesCalc
{
    public class DeleteSimulacaoCalcHandler : IRequestHandler<DeleteSimulacaoCalcCommand, DeleteApiResponse>
    {
        private readonly IBaseRepository<SimulacaoCalc> _simulacaoCalcRepository;

        public DeleteSimulacaoCalcHandler(IBaseRepository<SimulacaoCalc> simulacaoCalcRepository)
        {
            _simulacaoCalcRepository = simulacaoCalcRepository;
        }

        public async Task<DeleteApiResponse> Handle(DeleteSimulacaoCalcCommand request, CancellationToken cancellationToken)
        {
            var simulacaoCalc = await _simulacaoCalcRepository.GetByIdAsync(request.Id) ?? throw new SimulacaoCalcNotFoundException(request.Id);

            _simulacaoCalcRepository.Delete(simulacaoCalc);
            await _simulacaoCalcRepository.SaveChangesAsync();

            return new DeleteApiResponse { Message = "Simulação de cálculo excluída com sucesso." };
        }
    }
}
