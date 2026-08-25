using NCalc;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Application.Helpers
{
    public class ExpressionHelper
    {
        /// <summary>
        /// Placeholder de indicador composto: <c>{@PME}</c> (tag do catálogo).
        /// Distinto de códigos contábeis <c>{1.01.03}</c>.
        /// </summary>
        private static readonly Regex RefIndicadorRegex = new(
            @"\{@([^}]+)\}",
            RegexOptions.Compiled);

        private static readonly Regex CodigoContabilRegex = new(
            @"\{(?!@)([^}]+)\}",
            RegexOptions.Compiled);

        public static bool TemReferenciaIndicador(string? formula) =>
            !string.IsNullOrWhiteSpace(formula) && RefIndicadorRegex.IsMatch(formula);

        /// <summary>
        /// Substitui <c>{@TAG}</c> pelos valores já calculados no exercício (chave = tag).
        /// Tag ausente no mapa ou valor nulo (alerta) vira <c>0</c>.
        /// </summary>
        public static string SubstituirReferenciasIndicadores(
            string formula,
            IReadOnlyDictionary<string, double?> resultadosIndicadores)
        {
            if (string.IsNullOrWhiteSpace(formula))
                return "0";

            return RefIndicadorRegex.Replace(formula, match =>
            {
                var nome = match.Groups[1].Value.Trim();
                if (TryObterResultadoIndicador(resultadosIndicadores, nome, out var valor) && valor.HasValue)
                    return valor.Value.ToString(CultureInfo.InvariantCulture);

                return "0";
            });
        }

        private static bool TryObterResultadoIndicador(
            IReadOnlyDictionary<string, double?> map,
            string nome,
            out double? valor)
        {
            if (map.TryGetValue(nome, out valor))
                return true;

            foreach (var kv in map)
            {
                if (kv.Key.Equals(nome, StringComparison.OrdinalIgnoreCase))
                {
                    valor = kv.Value;
                    return true;
                }
            }

            valor = null;
            return false;
        }

        public static string SubstituirCodigos(string formula, Dictionary<string, double> valores)
        {
            if (string.IsNullOrWhiteSpace(formula))
                return "0";

            return CodigoContabilRegex.Replace(formula, match =>
            {
                var codigoOriginal = match.Groups[1].Value.Trim();

                if (valores.TryGetValue(codigoOriginal, out var valor))
                    return valor.ToString(CultureInfo.InvariantCulture);

                return "0";
            });
        }

        /// <summary>
        /// Resolve referências a indicadores (se houver) e depois códigos contábeis.
        /// </summary>
        public static string SubstituirFormula(
            string formula,
            Dictionary<string, double> valoresContabeis,
            IReadOnlyDictionary<string, double?>? resultadosIndicadores = null)
        {
            var expressao = formula;
            if (resultadosIndicadores != null && TemReferenciaIndicador(expressao))
                expressao = SubstituirReferenciasIndicadores(expressao, resultadosIndicadores);

            return SubstituirCodigos(expressao, valoresContabeis);
        }

        private static string ResolverExpressoesComDivisaoSegura(string expressao)
        {
            bool alterou;

            do
            {
                alterou = false;

                expressao = Regex.Replace(
                    expressao,
                    @"\(([\d\.\+\-\*/\s]+)\)",
                    match =>
                    {
                        try
                        {
                            string sub = match.Groups[1].Value.Trim();

                            /*
                            REGRA PRINCIPAL:
                            se existir divisão por zero,
                            retorna 0 antes do Evaluate()
                            */

                            if (TemDivisaoPorZero(sub))
                            {
                                alterou = true;
                                return "0";
                            }

                            var e = new Expression(sub);

                            var valor = Convert.ToDouble(
                                e.Evaluate(),
                                CultureInfo.InvariantCulture
                            );

                            if (double.IsNaN(valor) || double.IsInfinity(valor))
                            {
                                alterou = true;
                                return "0";
                            }

                            alterou = true;

                            return valor.ToString(
                                CultureInfo.InvariantCulture
                            );
                        }
                        catch
                        {
                            return match.Value;
                        }
                    }
                );

            } while (alterou);

            return expressao;
        }

        private static bool TemDivisaoPorZero(string expressao)
        {
            /*
            detecta:
            10 / 0
            150 / 0.00
            999 / 0.0000
            */

            return Regex.IsMatch(
                expressao,
                @"/\s*0+(\.0+)?(\D|$)"
            );
        }

        private static string NormalizarNumero(string texto)
        {
            texto = Regex.Replace(
                texto,
                @"\d{1,3}(\.\d{3})+,\d+",
                m => m.Value.Replace(".", "").Replace(",", ".")
            );

            texto = Regex.Replace(
                texto,
                @"(?<!\d)(\d+,\d+)",
                m => m.Value.Replace(",", ".")
            );

            return texto;
        }

        public static double AvaliarExpressao(string expressao)
        {
            if (string.IsNullOrWhiteSpace(expressao))
                return 0;

            try
            {
                // Normaliza decimal caso venha com vírgula
                expressao = NormalizarNumero(expressao);

                // Resolve divisões por zero em expressões complexas
                expressao = ResolverExpressoesComDivisaoSegura(expressao);

                var e = new Expression(expressao);
                var resultado = Convert.ToDouble(e.Evaluate(), CultureInfo.InvariantCulture);

                if (double.IsNaN(resultado) || double.IsInfinity(resultado))
                {
                    Console.WriteLine("[DEBUG] Resultado inválido (NaN ou Infinity)");
                    return 0;
                }

                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Erro NCalc] Expressão inválida: {expressao}");
                Console.WriteLine($"Mensagem: {ex.Message}");
                return 0;
            }
        }


        public static double ProcessarFormula(
            string formula,
            Dictionary<string, double> valores,
            IReadOnlyDictionary<string, double?>? resultadosIndicadores = null)
        {
            var expressao = SubstituirFormula(formula, valores, resultadosIndicadores);
            return AvaliarExpressao(expressao);
        }
    }
}
