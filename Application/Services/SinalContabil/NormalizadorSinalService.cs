using Application.Helpers;
using Application.Interfaces;
using Domain.Enums;
using Domain.Models;

namespace Application.Services.SinalContabil
{
    /// <summary>
    /// Sinal da ECF: crédito (C) = positivo, débito (D) = negativo.
    /// Ativo do balanço (<c>1</c>/<c>1.*</c>): inverte D/C antes dessa regra.
    /// Sem indicador, permanece a magnitude.
    /// Grupo de custos <c>3.01.01.03</c> (somente ela): sempre magnitude positiva (ignora D/C).
    /// Exceções de magnitude absoluta por indicador (PMP e Cobertura de Juros) ficam no helper de fórmulas.
    /// </summary>
    public class NormalizadorSinalService : INormalizadorSinalService
    {
        private readonly IGrupoContabilResolver _grupoResolver;

        public NormalizadorSinalService(IGrupoContabilResolver grupoResolver)
        {
            _grupoResolver = grupoResolver;
        }

        public decimal Normalizar(ContaContabil conta)
        {
            return AplicarIndicadorEcf(conta.Codigo, conta.SaldoBruto, conta.IndicadorDC);
        }

        public decimal Normalizar(string? codigo, decimal? saldoBruto, char? indicador)
        {
            return AplicarIndicadorEcf(codigo, saldoBruto, InterpretarIndicador(indicador));
        }

        public ContaContabil CriarConta(string? codigo, decimal? saldoBruto, char? indicador)
        {
            return new ContaContabil
            {
                Codigo = codigo ?? string.Empty,
                SaldoBruto = Math.Abs(saldoBruto ?? 0m),
                IndicadorDC = ParseIndicador(indicador),
                Grupo = _grupoResolver.Resolver(codigo)
            };
        }

        public static IndicadorDC ParseIndicador(char? indicador)
        {
            return char.ToUpperInvariant(indicador ?? 'D') == 'C'
                ? IndicadorDC.Credito
                : IndicadorDC.Debito;
        }

        public static decimal AplicarIndicadorEcf(decimal? saldoBruto, char? indicador) =>
            AplicarIndicadorEcf(null, saldoBruto, InterpretarIndicador(indicador));

        public static decimal AplicarIndicadorEcf(string? codigo, decimal? saldoBruto, char? indicador) =>
            AplicarIndicadorEcf(codigo, saldoBruto, InterpretarIndicador(indicador));

        public static decimal AplicarIndicadorEcf(decimal? saldoBruto, IndicadorDC? indicador) =>
            AplicarIndicadorEcf(null, saldoBruto, indicador);

        public static decimal AplicarIndicadorEcf(string? codigo, decimal? saldoBruto, IndicadorDC? indicador)
        {
            var magnitude = Math.Abs(saldoBruto ?? 0m);
            if (SaldoContabilHelper.EhContaCustoSemprePositiva(codigo))
                return magnitude;

            var dc = InverterDcSeAtivo(codigo, indicador);
            if (dc is null)
                return magnitude;

            return dc == IndicadorDC.Credito ? magnitude : -magnitude;
        }

        /// <summary>
        /// Ativo <c>1.*</c>: C da ECF vira D e D vira C, depois C = + e D = −.
        /// </summary>
        public static IndicadorDC? InverterDcSeAtivo(string? codigo, IndicadorDC? indicador)
        {
            if (indicador is null || !SaldoContabilHelper.EhContaAtivoBalanco(codigo))
                return indicador;

            return indicador == IndicadorDC.Credito ? IndicadorDC.Debito : IndicadorDC.Credito;
        }

        public static IndicadorDC? InterpretarIndicador(char? indicador)
        {
            if (!indicador.HasValue)
                return null;

            var c = char.ToUpperInvariant(indicador.Value);
            if (c == 'C')
                return IndicadorDC.Credito;
            if (c == 'D')
                return IndicadorDC.Debito;
            return null;
        }
    }
}
