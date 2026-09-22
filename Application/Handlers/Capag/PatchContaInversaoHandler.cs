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
    public class PatchContaInversaoHandler : IRequestHandler<PatchContaInversaoCommand, GetApiResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<ContaGreInversao> _inversaoRepository;
        private readonly GreBuilderService _greBuilder;
        private readonly ICurrentUserService _currentUser;

        public PatchContaInversaoHandler(
            IBaseRepository<Empresa> empresaRepository,
            IBaseRepository<ContaGreInversao> inversaoRepository,
            GreBuilderService greBuilder,
            ICurrentUserService currentUser)
        {
            _empresaRepository = empresaRepository;
            _inversaoRepository = inversaoRepository;
            _greBuilder = greBuilder;
            _currentUser = currentUser;
        }

        public async Task<GetApiResponse> Handle(PatchContaInversaoCommand request, CancellationToken cancellationToken)
        {
            var existe = await _empresaRepository.AnyAsync(e => e.IdEmpresa == request.EmpresaId && e.DeletedAt == null);
            if (!existe)
                throw new EmpresaNotFoundException(request.EmpresaId);

            var codigo = SaldoContabilHelper.NormalizarCodigo(request.CodigoConta);
            var atualizadoPor = _currentUser.UserId?.ToString() ?? _currentUser.Email ?? "sistema";
            var ano = request.Request.Ano;

            var existente = await _inversaoRepository.GetFirstOrDefaultAsync(x =>
                x.EmpresaId == request.EmpresaId && x.CodigoConta == codigo && x.Ano == ano);

            if (!request.Request.Invertido)
            {
                if (existente != null)
                {
                    _inversaoRepository.Delete(existente);
                    await _inversaoRepository.SaveChangesAsync();
                }
            }
            else if (existente == null)
            {
                existente = new ContaGreInversao
                {
                    EmpresaId = request.EmpresaId,
                    IdTenant = _currentUser.TenantId ?? 0,
                    CodigoConta = codigo,
                    Ano = ano,
                    Invertido = true,
                    Justificativa = request.Request.Justificativa,
                    AtualizadoEm = DateTimeHelper.GetDateTimeNow(),
                    AtualizadoPor = atualizadoPor
                };
                await _inversaoRepository.AddAsync(existente);
                await _inversaoRepository.SaveChangesAsync();
            }
            else
            {
                existente.Invertido = true;
                if (request.Request.Justificativa != null)
                    existente.Justificativa = request.Request.Justificativa;
                existente.AtualizadoEm = DateTimeHelper.GetDateTimeNow();
                existente.AtualizadoPor = atualizadoPor;
                _inversaoRepository.Update(existente);
                await _inversaoRepository.SaveChangesAsync();
            }

            var gre = await _greBuilder.ConstruirGre(request.EmpresaId, cancellationToken);

            return new GetApiResponse
            {
                Message = request.Request.Invertido
                    ? "Sinal invertido com sucesso"
                    : "Inversão de sinal removida com sucesso",
                Data = new ContaGreAjusteResponse
                {
                    IdEmpresa = request.EmpresaId,
                    CodigoConta = codigo,
                    Gre = gre
                }
            };
        }
    }
}
