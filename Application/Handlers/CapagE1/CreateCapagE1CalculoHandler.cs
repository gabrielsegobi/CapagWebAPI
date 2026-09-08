using Application.Commands.CapagE1;
using Application.Exceptions.CapagE1;
using Application.Exceptions.Empresas;
using Application.Helpers;
using Domain.Contracts.CapagE1;
using Domain.Entities;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.CapagE1
{
    public class CreateCapagE1CalculoHandler
        : IRequestHandler<CreateCapagE1CalculoCommand, CapagE1CalculoPayload>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<CapagE1Calculo> _calculoRepository;
        private readonly ICurrentUserService _currentUser;

        public CreateCapagE1CalculoHandler(
            IBaseRepository<Empresa> empresaRepository,
            IBaseRepository<CapagE1Calculo> calculoRepository,
            ICurrentUserService currentUser)
        {
            _empresaRepository = empresaRepository;
            _calculoRepository = calculoRepository;
            _currentUser = currentUser;
        }

        public async Task<CapagE1CalculoPayload> Handle(
            CreateCapagE1CalculoCommand command,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUser.TenantId
                ?? throw new Exceptions.ValidationException(["Tenant não informado."]);

            var idEmpresa = command.Request.IdEmpresa;
            var empresa = await _empresaRepository.GetByIdAsync(idEmpresa);
            if (empresa == null || empresa.IdTenant != tenantId)
                throw new EmpresaNotFoundException(idEmpresa);

            var modelo = CapagE1CalculoHelper.NormalizeModelo(command.Request.Modelo);

            var exists = await _calculoRepository.AnyAsync(
                c => c.IdEmpresa == idEmpresa && c.Modelo == modelo);
            if (exists)
                throw new CapagE1CalculoAlreadyExistsException(idEmpresa, modelo);

            var now = DateTimeHelper.GetDateTimeNow();
            var payload = CapagE1CalculoHelper.PrepareForStorage(
                command.Request, idEmpresa, modelo, id: null, atualizadoEm: now);

            var entity = new CapagE1Calculo
            {
                IdEmpresa = idEmpresa,
                IdTenant = tenantId,
                Modelo = modelo,
                PayloadJson = CapagE1CalculoHelper.SerializePayload(payload),
                DateCreate = now,
                DateUpdate = now,
                IdUsuario = _currentUser.UserId
            };

            await _calculoRepository.AddAsync(entity);
            await _calculoRepository.SaveChangesAsync();

            return CapagE1CalculoHelper.ToResponse(entity);
        }
    }
}
