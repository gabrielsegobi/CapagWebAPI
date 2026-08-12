using Application.Commands.CapagCalculadoraResultados;
using Application.Exceptions.CapagCalculadoraResultados;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.CapagCalculadoraResultados
{
    public class DeleteCapagCalculadoraResultadoHandler : IRequestHandler<DeleteCapagCalculadoraResultadoCommand, DeleteApiResponse>
    {
        private readonly IBaseRepository<CapagCalculadoraResultado> _baseRepository;

        public DeleteCapagCalculadoraResultadoHandler(IBaseRepository<CapagCalculadoraResultado> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<DeleteApiResponse> Handle(DeleteCapagCalculadoraResultadoCommand request, CancellationToken cancellationToken)
        {
            var entity = await _baseRepository.GetByIdAsync(request.Id)
                ?? throw new CapagCalculadoraResultadoNotFoundException(request.Id);

            _baseRepository.Delete(entity);
            await _baseRepository.SaveChangesAsync();

            return new DeleteApiResponse { Message = "Resultado da calculadora CAPAG deletado com sucesso." };
        }
    }
}
