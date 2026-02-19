using Application.Commands.Operations;
using Application.Exceptions.Empresas;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Handlers.Operations
{
    public class CreateOperationHandler : IRequestHandler<CreateOperationCommand, long>
    {
        private readonly IBaseRepository<Operation> _operationRepository;
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateOperationHandler(IBaseRepository<Operation> operationRepository, IBaseRepository<Empresa> empresaRepository, ICurrentUserService currentUserService)
        {
            _operationRepository = operationRepository;
            _empresaRepository = empresaRepository;
            _currentUserService = currentUserService;
        }

        public async Task<long> Handle(CreateOperationCommand request, CancellationToken cancellationToken)
        {

            var empresa = await _empresaRepository.GetByIdAsync(request.IdEmpresa) ?? throw new EmpresaNotFoundException(request.IdEmpresa);

            var idUsuario = _currentUserService.UserId ?? 1;
            var idTenant = _currentUserService.TenantId ?? 1;

            var operation = new Operation("Importaçaõ de ecf", idUsuario, request.IdEmpresa, idTenant, empresa.Cnpj);

            var id = await _operationRepository.AddAsyncAndGetId(operation);
            await _operationRepository.SaveChangesAsync();
            

            

            return id;
        }
    }
}
