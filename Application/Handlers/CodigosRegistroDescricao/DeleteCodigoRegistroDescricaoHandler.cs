using Application.Commands.CodigosRegistroDescricao;
using Application.Exceptions.CodigosRegistroDescricao;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.CodigosRegistroDescricao
{
    public class DeleteCodigoRegistroDescricaoHandler : IRequestHandler<DeleteCodigoRegistroDescricaoCommand, DeleteApiResponse>
    {
        private readonly IBaseRepository<CodigoRegistroDescricao> _baseRepository;

        public DeleteCodigoRegistroDescricaoHandler(IBaseRepository<CodigoRegistroDescricao> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<DeleteApiResponse> Handle(DeleteCodigoRegistroDescricaoCommand request, CancellationToken cancellationToken)
        {
            var entity = await _baseRepository.GetByIdAsync(request.Id)
                ?? throw new CodigoRegistroDescricaoNotFoundException(request.Id);

            _baseRepository.Delete(entity);
            await _baseRepository.SaveChangesAsync();

            return new DeleteApiResponse { Message = "Código de registro descrição deletado com sucesso." };
        }
    }
}
