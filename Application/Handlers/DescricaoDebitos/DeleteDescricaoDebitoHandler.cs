using Application.Commands.DescricaoDebitos;
using Application.Exceptions.DescricaoDebitos;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.DescricaoDebitos
{
    public class DeleteDescricaoDebitoHandler : IRequestHandler<DeleteDescricaoDebitoCommand, DeleteApiResponse>
    {
        private readonly IBaseRepository<DescricaoDebito> _baseRepository;


        public static string DeleteMessage = "Débito de intervalo deletada com sucesso.";
        public DeleteDescricaoDebitoHandler(IBaseRepository<DescricaoDebito> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<DeleteApiResponse> Handle(DeleteDescricaoDebitoCommand request, CancellationToken cancellationToken)
        {
            var debito = _baseRepository.GetByIdAsync(request.Id).Result
                ?? throw new DescricaoDebitoNotFoundException(request.Id);

            _baseRepository.Delete(debito);
            await _baseRepository.SaveChangesAsync();

            return new DeleteApiResponse { Message = DeleteMessage };
        }
    }
}
