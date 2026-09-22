using Application.Commands.Capag;
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
    public class CreateContaGreManualHandler : IRequestHandler<CreateContaGreManualCommand, GetApiResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<ContaGreManual> _manualRepository;
        private readonly GreBuilderService _greBuilder;
        private readonly ICurrentUserService _currentUser;

        public CreateContaGreManualHandler(
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

        public async Task<GetApiResponse> Handle(CreateContaGreManualCommand request, CancellationToken cancellationToken)
        {
            var existe = await _empresaRepository.AnyAsync(e => e.IdEmpresa == request.EmpresaId && e.DeletedAt == null);
            if (!existe)
                throw new EmpresaNotFoundException(request.EmpresaId);

            var atualizadoPor = _currentUser.UserId?.ToString() ?? _currentUser.Email ?? "sistema";
            var codigo = string.IsNullOrWhiteSpace(request.Request.CodigoConta)
                ? GreAjusteHelper.GerarCodigoManual()
                : SaldoContabilHelper.NormalizarCodigo(request.Request.CodigoConta);

            var entity = new ContaGreManual
            {
                EmpresaId = request.EmpresaId,
                IdTenant = _currentUser.TenantId ?? 0,
                CodigoConta = codigo,
                CodigoPai = string.IsNullOrWhiteSpace(request.Request.CodigoPai)
                    ? null
                    : SaldoContabilHelper.NormalizarCodigo(request.Request.CodigoPai),
                Descricao = request.Request.Descricao.Trim(),
                Tipo = GreAjusteHelper.NormalizarTipo(request.Request.Tipo),
                UsarMedia = request.Request.UsarMedia,
                Justificativa = request.Request.Justificativa,
                ValoresJson = GreAjusteHelper.SerializeValores(request.Request.Valores),
                AtualizadoEm = DateTimeHelper.GetDateTimeNow(),
                AtualizadoPor = atualizadoPor
            };

            await _manualRepository.AddAsync(entity);
            await _manualRepository.SaveChangesAsync();

            var gre = await _greBuilder.ConstruirGre(request.EmpresaId, cancellationToken);

            return new GetApiResponse
            {
                Message = "Conta manual incluída com sucesso",
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
