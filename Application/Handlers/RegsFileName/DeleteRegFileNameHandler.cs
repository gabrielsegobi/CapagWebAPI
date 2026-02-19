using Application.Commands.RegsFileName;
using Application.Exceptions.RegsFileName;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegsFileName
{
    public class DeleteRegFileNameHandler : IRequestHandler<DeleteRegFileNameCommnad, CreateApiResponse>
    {
        private readonly IBaseRepository<RegFileName> _regFileNameRepository;

        public DeleteRegFileNameHandler(IBaseRepository<RegFileName> regFileNameRepository)
        {
            _regFileNameRepository = regFileNameRepository;
        }

        public async Task<CreateApiResponse> Handle(DeleteRegFileNameCommnad request, CancellationToken cancellationToken)
        {
            var reg = await _regFileNameRepository.GetByIdAsync(request.Id)
                ?? throw new RegFileNameNotFoundException(request.Id);

            _regFileNameRepository.Delete(reg);
            await _regFileNameRepository.SaveChangesAsync();

            return new CreateApiResponse { Message = "Arquivo deletado com sucesso" };
        }
    }
}
