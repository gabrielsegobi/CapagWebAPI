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
    public class PatchContaExclusaoHandler : IRequestHandler<PatchContaExclusaoCommand, GetApiResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<ContaExclusaoConfig> _exclusaoRepository;
        private readonly GreBuilderService _greBuilder;
        private readonly ICurrentUserService _currentUser;

        public PatchContaExclusaoHandler(
            IBaseRepository<Empresa> empresaRepository,
            IBaseRepository<ContaExclusaoConfig> exclusaoRepository,
            GreBuilderService greBuilder,
            ICurrentUserService currentUser)
        {
            _empresaRepository = empresaRepository;
            _exclusaoRepository = exclusaoRepository;
            _greBuilder = greBuilder;
            _currentUser = currentUser;
        }

        public async Task<GetApiResponse> Handle(PatchContaExclusaoCommand request, CancellationToken cancellationToken)
        {
            var existe = await _empresaRepository.AnyAsync(e => e.IdEmpresa == request.EmpresaId && e.DeletedAt == null);
            if (!existe)
                throw new EmpresaNotFoundException(request.EmpresaId);

            var codigo = SaldoContabilHelper.NormalizarCodigo(request.CodigoConta);
            var atualizadoPor = _currentUser.UserId?.ToString() ?? _currentUser.Email ?? "sistema";

            var existente = await _exclusaoRepository.GetFirstOrDefaultAsync(x =>
                x.EmpresaId == request.EmpresaId && x.CodigoConta == codigo);

            if (existente == null)
            {
                existente = new ContaExclusaoConfig
                {
                    EmpresaId = request.EmpresaId,
                    IdTenant = _currentUser.TenantId ?? 0,
                    CodigoConta = codigo,
                    Excluida = request.Request.Excluida,
                    Justificativa = request.Request.Justificativa,
                    AtualizadoEm = DateTimeHelper.GetDateTimeNow(),
                    AtualizadoPor = atualizadoPor
                };
                await _exclusaoRepository.AddAsync(existente);
            }
            else
            {
                existente.Excluida = request.Request.Excluida;
                existente.Justificativa = request.Request.Justificativa;
                existente.AtualizadoEm = DateTimeHelper.GetDateTimeNow();
                existente.AtualizadoPor = atualizadoPor;
                _exclusaoRepository.Update(existente);
            }

            await _exclusaoRepository.SaveChangesAsync();

            var gre = await _greBuilder.ConstruirGre(request.EmpresaId, cancellationToken);

            return new GetApiResponse
            {
                Message = request.Request.Excluida
                    ? "Conta excluída do cálculo com sucesso"
                    : "Conta reincluída no cálculo com sucesso",
                Data = new ContaExclusaoResponse
                {
                    IdEmpresa = request.EmpresaId,
                    CodigoConta = codigo,
                    Excluida = existente.Excluida,
                    Justificativa = existente.Justificativa,
                    AtualizadoEm = existente.AtualizadoEm,
                    AtualizadoPor = existente.AtualizadoPor,
                    Gre = gre
                }
            };
        }
    }
}
