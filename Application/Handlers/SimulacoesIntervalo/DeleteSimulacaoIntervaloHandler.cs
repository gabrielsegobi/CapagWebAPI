using Application.Commands.SimulacoesIntervalo;
using Application.Exceptions.SimulacoesIntervalo;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.SimulacoesIntervalo
{
    public class DeleteSimulacaoIntervaloHandler : IRequestHandler<DeleteSimulacaoIntervaloCommand, DeleteApiResponse>
    {
        private readonly IBaseRepository<SimulacaoIntervalo> _baseRepository;

        public static string DeleteMessage = "Simulação de intervalo deletada com sucesso.";
        public DeleteSimulacaoIntervaloHandler(IBaseRepository<SimulacaoIntervalo> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<DeleteApiResponse> Handle(DeleteSimulacaoIntervaloCommand request, CancellationToken cancellationToken)
        {
            var calc = await _baseRepository.GetByIdAsync(request.Id)
                ?? throw new SimulacaoIntervaloNotFoundException(request.Id);

            _baseRepository.Delete(calc);
            await _baseRepository.SaveChangesAsync();

            return new DeleteApiResponse { Message = DeleteMessage };
        }
    }
}
