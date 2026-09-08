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
    public class UpdateCapagE1CalculoHandler
        : IRequestHandler<UpdateCapagE1CalculoCommand, CapagE1CalculoPayload>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<CapagE1Calculo> _calculoRepository;
        private readonly ICurrentUserService _currentUser;

        public UpdateCapagE1CalculoHandler(
            IBaseRepository<Empresa> empresaRepository,
            IBaseRepository<CapagE1Calculo> calculoRepository,
            ICurrentUserService currentUser)
        {
            _empresaRepository = empresaRepository;
            _calculoRepository = calculoRepository;
            _currentUser = currentUser;
        }

        public async Task<CapagE1CalculoPayload> Handle(
            UpdateCapagE1CalculoCommand command,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUser.TenantId
                ?? throw new Exceptions.ValidationException(["Tenant não informado."]);

            var entity = await _calculoRepository.GetByIdAsync(command.Id)
                ?? throw new CapagE1CalculoNotFoundException(command.Id);

            if (entity.IdTenant != tenantId)
                throw new CapagE1CalculoNotFoundException(command.Id);

            var idEmpresa = command.Request.IdEmpresa > 0
                ? command.Request.IdEmpresa
                : entity.IdEmpresa;

            if (idEmpresa != entity.IdEmpresa)
                throw new Exceptions.ValidationException(
                    ["id_empresa do body não corresponde ao cálculo persistido."]);

            var empresa = await _empresaRepository.GetByIdAsync(idEmpresa);
            if (empresa == null || empresa.IdTenant != tenantId)
                throw new EmpresaNotFoundException(idEmpresa);

            var modelo = CapagE1CalculoHelper.NormalizeModelo(
                string.IsNullOrWhiteSpace(command.Request.Modelo)
                    ? entity.Modelo
                    : command.Request.Modelo);

            if (modelo != entity.Modelo)
            {
                var conflict = await _calculoRepository.AnyAsync(
                    c => c.IdEmpresa == idEmpresa && c.Modelo == modelo && c.Id != entity.Id);
                if (conflict)
                    throw new CapagE1CalculoAlreadyExistsException(idEmpresa, modelo);
            }

            var now = DateTimeHelper.GetDateTimeNow();
            var payload = CapagE1CalculoHelper.PrepareForStorage(
                command.Request, idEmpresa, modelo, entity.Id, now);

            entity.Modelo = modelo;
            entity.PayloadJson = CapagE1CalculoHelper.SerializePayload(payload);
            entity.DateUpdate = now;
            entity.IdUsuario = _currentUser.UserId;

            _calculoRepository.Update(entity);
            await _calculoRepository.SaveChangesAsync();

            return CapagE1CalculoHelper.ToResponse(entity);
        }
    }
}
