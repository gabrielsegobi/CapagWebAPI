using Application.Commands.Empresas;
using Application.Exceptions.Empresas;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Empresas
{
    public class DeleteEmpresaHandler : IRequestHandler<DeleteEmpresaCommand, DeleteApiResponse>
    {
        private readonly IBaseRepository<Empresa> _baseRepository;
        public DeleteEmpresaHandler(IBaseRepository<Empresa> baseRepository)
        {
            _baseRepository = baseRepository;
        }
        public async Task<DeleteApiResponse> Handle(DeleteEmpresaCommand request, CancellationToken cancellationToken)
        {
            var empresa = await _baseRepository.GetByIdAsync(request.IdEmpresa);

            if (empresa == null)
            {
                throw new EmpresaNotFoundException(request.IdEmpresa);
            }

            empresa.DeletedAt = DateTimeHelper.GetDateTimeNow();

            _baseRepository.Update(empresa);
            await _baseRepository.SaveChangesAsync();
          
            return new DeleteApiResponse { Message = "Empresa deleteada com sucesso" };
        }
    }
}
