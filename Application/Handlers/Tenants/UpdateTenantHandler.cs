using Application.Commands.Tenants;
using Application.Exceptions.Tenants;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Tenants
{
    public class UpdateTenantHandler : IRequestHandler<UpdateTenantCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<Tenant> _baseRepository;
        private readonly IMapper _mapper;

        public UpdateTenantHandler(IBaseRepository<Tenant> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<UpdateApiResponse> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
        {
            var tenant = await _baseRepository.GetByIdAsync(request.Id) ?? throw new TenantNotFoundException(request.Id);

            var tenanttoupdate = _mapper.Map(request.UpdateTenantRequest, tenant);

            _baseRepository.Update(tenanttoupdate);
            await _baseRepository.SaveChangesAsync();

            return new UpdateApiResponse { Message = "Tenant atualizado com sucesso" };
        }
    }
}
