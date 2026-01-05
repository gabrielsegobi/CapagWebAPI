using NCalc;
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

                var codigoSemI = Regex.Replace(codigoOriginal, @"\[I\]", "", RegexOptions.IgnoreCase);
                if (valores.TryGetValue(codigoSemI, out valor))
                    return valor.ToString(System.Globalization.CultureInfo.InvariantCulture);

                return "0";
            });

        }

        public static double AvaliarExpressao(string expressao)
        {
            if (string.IsNullOrWhiteSpace(expressao))
                return 0;

            try
            {
                if (expressao.Contains("/ 0") || expressao.Contains("/0"))
                {
                    Console.WriteLine("[DEBUG] Detectado denominador zero → valorCalculado = 0");
                    return 0;
                }
                var e = new Expression(expressao);
                return Convert.ToDouble(e.Evaluate());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Erro NCalc] Expressão inválida: {expressao}");
                Console.WriteLine($"Mensagem: {ex.Message}");
                return double.NaN;
            }
        }


        public static double ProcessarFormula(string formula, Dictionary<string, double> valores)
        {
            var expressao = SubstituirCodigos(formula, valores);
            return AvaliarExpressao(expressao);
        }
    }
}
