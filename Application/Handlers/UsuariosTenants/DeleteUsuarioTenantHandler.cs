using Application.Commands.UsuariosTenants;
using Application.Exceptions.UsuariosTenant;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.UsuariosTenants
{
    public class DeleteUsuarioTenantHandler : IRequestHandler<DeleteUsuarioTenantCommand, DeleteApiResponse>
    {
        private readonly IBaseRepository<UsuarioTenant> _baseRepository;

        public DeleteUsuarioTenantHandler(IBaseRepository<UsuarioTenant> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<DeleteApiResponse> Handle(DeleteUsuarioTenantCommand request, CancellationToken cancellationToken)
        {
            var usuariotenant = await _baseRepository.GetByIdAsync(request.Id);

            if (usuariotenant == null)
            {
                throw new UsuarioTenantNotFoundException(request.Id);
            }

            usuariotenant.DeletedAt = DateTimeHelper.GetDateTimeNow();

            _baseRepository.Update(usuariotenant);
            await _baseRepository.SaveChangesAsync();

            return new DeleteApiResponse { Message = "Usuário tenant Deletado com sucesso" };
        }
    }
}
