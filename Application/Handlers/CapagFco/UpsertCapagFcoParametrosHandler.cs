using Application.Commands.CapagFco;
using Application.Exceptions.Empresas;
using Application.Helpers;
using Domain.Contracts.CapagFco;
using Domain.Entities;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.CapagFco
{
    public class UpsertCapagFcoParametrosHandler
        : IRequestHandler<UpsertCapagFcoParametrosCommand, CapagFcoParametrosResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<CapagFcoParametroEmpresa> _parametroRepository;
        private readonly ICurrentUserService _currentUser;

        public UpsertCapagFcoParametrosHandler(
            IBaseRepository<Empresa> empresaRepository,
            IBaseRepository<CapagFcoParametroEmpresa> parametroRepository,
            ICurrentUserService currentUser)
        {
            _empresaRepository = empresaRepository;
            _parametroRepository = parametroRepository;
            _currentUser = currentUser;
        }

        public async Task<CapagFcoParametrosResponse> Handle(
            UpsertCapagFcoParametrosCommand command,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUser.TenantId
                ?? throw new Exceptions.ValidationException(["Tenant não informado."]);

            var empresa = await _empresaRepository.GetByIdAsync(command.IdEmpresa);
            if (empresa == null || empresa.IdTenant != tenantId)
                throw new EmpresaNotFoundException(command.IdEmpresa);

            var bloco = CapagFcoParametrosHelper.NormalizeBloco(command.Request.Bloco);
            var excecoesJson = CapagFcoParametrosHelper.SerializeExcecoes(
                CapagFcoParametrosHelper.NormalizeExcecoes(command.Request.Excecoes));
            var versaoBase = CapagFcoParametrosHelper.NormalizeVersaoBase(command.Request.VersaoBase);
            var now = DateTimeHelper.GetDateTimeNow();
            var idUsuario = _currentUser.UserId;

            var entity = await _parametroRepository.GetFirstOrDefaultAsync(
                p => p.IdEmpresa == command.IdEmpresa);

            if (entity == null)
            {
                entity = new CapagFcoParametroEmpresa
                {
                    IdEmpresa = command.IdEmpresa,
                    IdTenant = tenantId,
                    ExcecoesL100Json = bloco == CapagFcoParametrosHelper.BlocoL100
                        ? excecoesJson
                        : CapagFcoParametrosHelper.EmptyJsonObject,
                    ExcecoesL300Json = bloco == CapagFcoParametrosHelper.BlocoL300
                        ? excecoesJson
                        : CapagFcoParametrosHelper.EmptyJsonObject,
                    BaseVersaoHash = versaoBase,
                    CreatedAt = now,
                    UpdatedAt = now,
                    IdUsuario = idUsuario
                };

                await _parametroRepository.AddAsync(entity);
            }
            else
            {
                if (bloco == CapagFcoParametrosHelper.BlocoL100)
                    entity.ExcecoesL100Json = excecoesJson;
                else
                    entity.ExcecoesL300Json = excecoesJson;

                entity.BaseVersaoHash = versaoBase;
                entity.UpdatedAt = now;
                entity.IdUsuario = idUsuario;
                _parametroRepository.Update(entity);
            }

            await _parametroRepository.SaveChangesAsync();

            return CapagFcoParametrosHelper.ToResponse(command.IdEmpresa, entity);
        }
    }
}
