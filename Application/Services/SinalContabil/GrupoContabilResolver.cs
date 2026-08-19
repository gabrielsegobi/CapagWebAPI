using Application.Helpers;
using Application.Interfaces;
using Domain.Enums;

namespace Application.Services.SinalContabil
{
    public class GrupoContabilResolver : IGrupoContabilResolver
    {
        public GrupoContabil Resolver(string? codigo)
        {
            var n = SaldoContabilHelper.NormalizarCodigo(codigo);
            if (string.IsNullOrEmpty(n))
                return GrupoContabil.Ativo;

            if (n.StartsWith("1", StringComparison.Ordinal))
                return GrupoContabil.Ativo;

            if (n == "2.03" || n.StartsWith("2.03.", StringComparison.Ordinal))
                return GrupoContabil.PatrimonioLiquido;

            if (n.StartsWith("2", StringComparison.Ordinal))
                return GrupoContabil.Passivo;

            if (n.StartsWith("3", StringComparison.Ordinal))
                return EhReceitaDre(n) ? GrupoContabil.Receita : GrupoContabil.Despesa;

            return GrupoContabil.Ativo;
        }

        /// <summary>
        /// Natureza de resultado/receita (C = positivo). Prefixos mais específicos primeiro
        /// para não classificar custos (3.01.01.03) como receita por causa de 3.01.01.
        /// </summary>
        private static bool EhReceitaDre(string codigo)
        {
            if (codigo is "3" or "3.01" or "3.01.01" or "3.01.01.02")
                return true;

            if (codigo == "3.01.01.01.02" || codigo.StartsWith("3.01.01.01.02.", StringComparison.Ordinal))
                return false;

            if (codigo == "3.01.01.01" || codigo.StartsWith("3.01.01.01.", StringComparison.Ordinal))
                return true;

            if (codigo.StartsWith("3.01.01.05", StringComparison.Ordinal))
                return true;

            if (codigo.StartsWith("3.01.01.11", StringComparison.Ordinal))
                return true;

            return false;
        }
    }
}
