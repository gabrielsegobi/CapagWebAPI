using Application.Commands.Tenants;
using Application.Exceptions.Tenants;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Tenants
{
    public class DeleteTenantHandler : IRequestHandler<DeleteTenantCommand, DeleteApiResponse>
    {
        private readonly IBaseRepository<Tenant> _baseRepository;
        public DeleteTenantHandler(IBaseRepository<Tenant> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<DeleteApiResponse> Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
        {
            var tenant = await _baseRepository.GetByIdAsync(request.Id);

            if (tenant == null)
            {
                throw new TenantNotFoundException(request.Id);
            }

            tenant.DeletedAt = DateTimeHelper.GetDateTimeNow();

            _baseRepository.Update(tenant);
            await _baseRepository.SaveChangesAsync();

            return new DeleteApiResponse { Message = "Tenant Deletado Com Sucesso" };
        }
    }
}
