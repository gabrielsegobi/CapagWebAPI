using Application.Commands.UsuariosTenants;
using Application.Exceptions.UsuariosTenant;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.UsuariosTenants
{
    public class UpdateUsuarioTenantHandler : IRequestHandler<UpdateUsuarioTenantCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<UsuarioTenant> _baseRepository;
        private readonly IMapper _mapper;

        public UpdateUsuarioTenantHandler(IBaseRepository<UsuarioTenant> baseRepository, IMapper mapepr)
        {
            _baseRepository = baseRepository;
            _mapper = mapepr;
        }

        public async Task<UpdateApiResponse> Handle(UpdateUsuarioTenantCommand request, CancellationToken cancellationToken)
        {
            var usuariotenant = await _baseRepository.GetByIdAsync(request.Id);

            if (usuariotenant == null)
            {
                throw new UsuarioTenantNotFoundException(request.Id);
            }

            var usuariotenantopdate = _mapper.Map(request.UpdateUsuarioTenantRequest, usuariotenant);

            _baseRepository.Update(usuariotenantopdate);
            await _baseRepository.SaveChangesAsync();

            return new UpdateApiResponse { Message = "Usuário tenant criado com sucesso" };
        }
    }
}
