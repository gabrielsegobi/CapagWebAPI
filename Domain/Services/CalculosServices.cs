namespace Domain.Services
{
    public static class  CalculosServices
    {

        public static decimal CalcularSubScore(decimal meta, decimal piorCaso, decimal valorCalculado)
        {
            try
            {
                var numerador = valorCalculado - piorCaso;
                var denominador = meta - piorCaso;

                if (denominador != 0m)
                {
                    var bruto = numerador / denominador;
                    var subScore = Math.Min(1m, Math.Max(0m, bruto));
                    return subScore * 100m;
                }
            }
            catch { }

            return 0m;
        }
        public static decimal CalcularIndiceTotal(IEnumerable<(decimal SubScore, decimal Peso)> indicadores)
        {
            decimal indiceTotal = 0m;
            foreach (var indicador in indicadores)
            {
                indiceTotal += indicador.SubScore * indicador.Peso;
            }
            return indiceTotal;
        }

        public static string CalcularClassificacao(decimal indiceTotal)
        {
            if (indiceTotal <= 45m)
                return "D";
            else if (indiceTotal <= 75m)
                return "C";
            else if (indiceTotal <= 90m)
                return "B";
            else
                return "A";
        }
    }
}
