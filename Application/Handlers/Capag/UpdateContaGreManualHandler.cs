using Application.Commands.Capag;
using Application.Exceptions.Capag;
using Application.Exceptions.Empresas;
using Application.Helpers;
using Application.Services.Capag;
using Domain.Contracts.Capag;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Capag
{
    public class UpdateContaGreManualHandler : IRequestHandler<UpdateContaGreManualCommand, GetApiResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<ContaGreManual> _manualRepository;
        private readonly GreBuilderService _greBuilder;
        private readonly ICurrentUserService _currentUser;

        public UpdateContaGreManualHandler(
            IBaseRepository<Empresa> empresaRepository,
            IBaseRepository<ContaGreManual> manualRepository,
            GreBuilderService greBuilder,
            ICurrentUserService currentUser)
        {
            _empresaRepository = empresaRepository;
            _manualRepository = manualRepository;
            _greBuilder = greBuilder;
            _currentUser = currentUser;
        }

        public async Task<GetApiResponse> Handle(UpdateContaGreManualCommand request, CancellationToken cancellationToken)
        {
            var existe = await _empresaRepository.AnyAsync(e => e.IdEmpresa == request.EmpresaId && e.DeletedAt == null);
            if (!existe)
                throw new EmpresaNotFoundException(request.EmpresaId);

            var entity = await _manualRepository.GetFirstOrDefaultAsync(x =>
                x.Id == request.Id && x.EmpresaId == request.EmpresaId)
                ?? throw new ContaGreManualNotFoundException(request.Id);

            var atualizadoPor = _currentUser.UserId?.ToString() ?? _currentUser.Email ?? "sistema";

            if (!string.IsNullOrWhiteSpace(request.Request.CodigoConta))
                entity.CodigoConta = SaldoContabilHelper.NormalizarCodigo(request.Request.CodigoConta);

            entity.CodigoPai = string.IsNullOrWhiteSpace(request.Request.CodigoPai)
                ? null
                : SaldoContabilHelper.NormalizarCodigo(request.Request.CodigoPai);
            entity.Descricao = request.Request.Descricao.Trim();
            entity.Tipo = GreAjusteHelper.NormalizarTipo(request.Request.Tipo);
            entity.UsarMedia = request.Request.UsarMedia;
            entity.Justificativa = request.Request.Justificativa;
            entity.ValoresJson = GreAjusteHelper.SerializeValores(request.Request.Valores);
            entity.AtualizadoEm = DateTimeHelper.GetDateTimeNow();
            entity.AtualizadoPor = atualizadoPor;

            _manualRepository.Update(entity);
            await _manualRepository.SaveChangesAsync();

            var gre = await _greBuilder.ConstruirGre(request.EmpresaId, cancellationToken);

            return new GetApiResponse
            {
                Message = "Conta manual atualizada com sucesso",
                Data = new ContaGreManualResponse
                {
                    IdEmpresa = request.EmpresaId,
                    Conta = GreAjusteHelper.ToDto(entity),
                    Gre = gre
                }
            };
        }
    }
}
