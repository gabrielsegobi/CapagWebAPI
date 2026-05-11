using NCalc;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Application.Helpers
{
    public class ExpressionHelper
    {

        public static string SubstituirCodigos(string formula, Dictionary<string, double> valores)
        {
            if (string.IsNullOrWhiteSpace(formula))
                return "0";

            return Regex.Replace(formula, @"\{([^}]+)\}", match =>
            {
                var codigoOriginal = match.Groups[1].Value.Trim();
                double valor;

                if (valores.TryGetValue(codigoOriginal, out valor))
                    return valor.ToString(System.Globalization.CultureInfo.InvariantCulture);

                // var codigoSemI = Regex.Replace(codigoOriginal, @"\[I\]", "", RegexOptions.IgnoreCase);
                // if (valores.TryGetValue(codigoSemI, out valor))
                //     return valor.ToString(System.Globalization.CultureInfo.InvariantCulture);

                return "0";
            });

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


        public static double ProcessarFormula(string formula, Dictionary<string, double> valores)
        {
            var expressao = SubstituirCodigos(formula, valores);
            return AvaliarExpressao(expressao);
        }
    }
}
