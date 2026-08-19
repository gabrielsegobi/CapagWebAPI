using Application.Helpers;
using Application.Interfaces;
using Application.Services.Demonstrativos;
using Domain.Constants;
using Domain.Contracts.PrlA;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.PrlA
{
    public interface IPrlAService
    {
        Task<PrlAResultadoDto> ObterAsync(long empresaId, int? ano, CancellationToken cancellationToken = default);
        Task<PatchPrlAResponse> AlterarBlocoAsync(long empresaId, string codigoConta, BlocoLiquidez bloco, string atualizadoPor, CancellationToken cancellationToken = default);
        Task<PatchPrlAResponse> AlterarDesagioAsync(long empresaId, string codigoConta, decimal percentualDesagio, string atualizadoPor, CancellationToken cancellationToken = default);
        decimal CalcularPassivoTotalAjustado(IEnumerable<ContaPrlADto> contas);
    }

    public class PrlAService : IPrlAService
    {
        private readonly DemonstrativoLeituraService _leitura;
        private readonly IBaseRepository<ContaBlocoOverride> _blocoRepository;
        private readonly IBaseRepository<ContaDesagioOverride> _desagioRepository;
        private readonly ICurrentUserService _currentUser;

        public PrlAService(
            DemonstrativoLeituraService leitura,
            IBaseRepository<ContaBlocoOverride> blocoRepository,
            IBaseRepository<ContaDesagioOverride> desagioRepository,
            ICurrentUserService currentUser)
        {
            _leitura = leitura;
            _blocoRepository = blocoRepository;
            _desagioRepository = desagioRepository;
            _currentUser = currentUser;
        }

        public async Task<PrlAResultadoDto> ObterAsync(long empresaId, int? ano, CancellationToken cancellationToken = default)
        {
            var contas = await MontarContasAsync(empresaId, ano, cancellationToken);
            var visiveis = FiltrarComValor(contas).ToList();

            return new PrlAResultadoDto
            {
                IdEmpresa = empresaId,
                Ano = ano,
                Contas = visiveis,
                Total = CalcularPassivoTotalAjustado(visiveis)
            };
        }

        public async Task<PatchPrlAResponse> AlterarBlocoAsync(
            long empresaId,
            string codigoConta,
            BlocoLiquidez bloco,
            string atualizadoPor,
            CancellationToken cancellationToken = default)
        {
            var codigo = SaldoContabilHelper.NormalizarCodigo(codigoConta);
            var original = ResolverBlocoOriginal(codigo);

            var existente = await _blocoRepository.GetFirstOrDefaultAsync(x =>
                x.EmpresaId == empresaId && x.CodigoConta == codigo);

            if (existente == null)
            {
                existente = new ContaBlocoOverride
                {
                    EmpresaId = empresaId,
                    IdTenant = _currentUser.TenantId ?? 0,
                    CodigoConta = codigo,
                    BlocoOriginal = original,
                    BlocoAjustado = bloco,
                    AtualizadoEm = DateTimeHelper.GetDateTimeNow(),
                    AtualizadoPor = atualizadoPor
                };
                await _blocoRepository.AddAsync(existente);
            }
            else
            {
                existente.BlocoOriginal = original;
                existente.BlocoAjustado = bloco;
                existente.AtualizadoEm = DateTimeHelper.GetDateTimeNow();
                existente.AtualizadoPor = atualizadoPor;
                _blocoRepository.Update(existente);
            }

            await _blocoRepository.SaveChangesAsync();

            var desagioManual = await _desagioRepository.GetFirstOrDefaultAsync(x =>
                x.EmpresaId == empresaId && x.CodigoConta == codigo);
            if (desagioManual != null)
            {
                _desagioRepository.Delete(desagioManual);
                await _desagioRepository.SaveChangesAsync();
            }

            return await MontarPatchResponse(empresaId, codigo, cancellationToken);
        }

        public async Task<PatchPrlAResponse> AlterarDesagioAsync(
            long empresaId,
            string codigoConta,
            decimal percentualDesagio,
            string atualizadoPor,
            CancellationToken cancellationToken = default)
        {
            var codigo = SaldoContabilHelper.NormalizarCodigo(codigoConta);

            var existente = await _desagioRepository.GetFirstOrDefaultAsync(x =>
                x.EmpresaId == empresaId && x.CodigoConta == codigo);

            if (existente == null)
            {
                existente = new ContaDesagioOverride
                {
                    EmpresaId = empresaId,
                    IdTenant = _currentUser.TenantId ?? 0,
                    CodigoConta = codigo,
                    PercentualDesagio = percentualDesagio,
                    AtualizadoEm = DateTimeHelper.GetDateTimeNow(),
                    AtualizadoPor = atualizadoPor
                };
                await _desagioRepository.AddAsync(existente);
            }
            else
            {
                existente.PercentualDesagio = percentualDesagio;
                existente.AtualizadoEm = DateTimeHelper.GetDateTimeNow();
                existente.AtualizadoPor = atualizadoPor;
                _desagioRepository.Update(existente);
            }

            await _desagioRepository.SaveChangesAsync();
            return await MontarPatchResponse(empresaId, codigo, cancellationToken);
        }

        public decimal CalcularPassivoTotalAjustado(IEnumerable<ContaPrlADto> contas)
        {
            return contas.Sum(c => c.SaldoAjustado ?? c.SaldoOriginal);
        }

        public static IEnumerable<ContaPrlADto> FiltrarComValor(IEnumerable<ContaPrlADto> contas) =>
            contas.Where(c => c.SaldoNormalizado != 0);

        public static BlocoLiquidez ResolverBlocoOriginal(string codigo)
        {
            var n = SaldoContabilHelper.NormalizarCodigo(codigo);

            if (n == "1.01.01" || n.StartsWith("1.01.01.", StringComparison.Ordinal))
                return BlocoLiquidez.A;

            if (n == "1.01" || n.StartsWith("1.01.", StringComparison.Ordinal) ||
                n == "2.01" || n.StartsWith("2.01.", StringComparison.Ordinal))
                return BlocoLiquidez.B;

            return BlocoLiquidez.C;
        }

        private async Task<PatchPrlAResponse> MontarPatchResponse(long empresaId, string codigo, CancellationToken cancellationToken)
        {
            var resultado = await ObterAsync(empresaId, null, cancellationToken);
            var conta = resultado.Contas.FirstOrDefault(c => c.Codigo == codigo)
                ?? (await MontarContasAsync(empresaId, null, cancellationToken))
                    .FirstOrDefault(c => c.Codigo == codigo)
                ?? new ContaPrlADto { Codigo = codigo };

            return new PatchPrlAResponse
            {
                IdEmpresa = empresaId,
                Conta = conta,
                Total = resultado.Total
            };
        }

        private async Task<List<ContaPrlADto>> MontarContasAsync(long empresaId, int? ano, CancellationToken cancellationToken)
        {
            var saldos = await _leitura.ObterSaldosNormalizadosAsync(empresaId, cancellationToken: cancellationToken);
            var balanco = saldos
                .Where(s => !s.IsDre)
                .Where(s => ano == null || s.Ano == ano)
                .ToList();

            var blocos = await _blocoRepository
                .Query(x => x.EmpresaId == empresaId)
                .ToListAsync(cancellationToken);

            var desagios = await _desagioRepository
                .Query(x => x.EmpresaId == empresaId)
                .ToListAsync(cancellationToken);

            var blocoPorCodigo = blocos.ToDictionary(x => x.CodigoConta, StringComparer.Ordinal);
            var desagioPorCodigo = desagios.ToDictionary(x => x.CodigoConta, StringComparer.Ordinal);

            var contas = new List<ContaPrlADto>();
            foreach (var saldo in balanco)
            {
                var codigo = SaldoContabilHelper.NormalizarCodigo(saldo.Codigo);
                var blocoOriginal = ResolverBlocoOriginal(codigo);
                blocoPorCodigo.TryGetValue(codigo, out var blocoOverride);
                desagioPorCodigo.TryGetValue(codigo, out var desagioOverride);

                var blocoEfetivo = blocoOverride?.BlocoAjustado ?? blocoOverride?.BlocoOriginal ?? blocoOriginal;
                var desagioManual = desagioOverride != null;
                var percentual = desagioManual
                    ? desagioOverride!.PercentualDesagio
                    : PrlAConstants.DesagioPadrao(blocoEfetivo);

                var saldoOriginal = saldo.SaldoNormalizado;
                var saldoAjustado = Math.Round(saldoOriginal * (1m - percentual / 100m), 2, MidpointRounding.AwayFromZero);

                contas.Add(new ContaPrlADto
                {
                    Codigo = codigo,
                    Descricao = saldo.Descricao,
                    Ano = saldo.Ano,
                    BlocoOriginal = blocoOverride?.BlocoOriginal ?? blocoOriginal,
                    BlocoAjustado = blocoOverride?.BlocoAjustado,
                    BlocoEfetivo = blocoEfetivo,
                    PercentualDesagio = percentual,
                    DesagioManual = desagioManual,
                    SaldoOriginal = saldoOriginal,
                    SaldoAjustado = saldoAjustado,
                    SaldoNormalizado = saldoOriginal
                });
            }

            return contas;
        }
    }
}
