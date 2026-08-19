using Application.Interfaces;

namespace Application.Services.SinalContabil
{
    /// <summary>
    /// Instância estática apenas para caminhos legados que ainda não recebem DI.
    /// A regra de sinal vive somente em <see cref="NormalizadorSinalService"/>.
    /// </summary>
    public static class NormalizadorSinalLocator
    {
        public static readonly IGrupoContabilResolver Grupo = new GrupoContabilResolver();
        public static readonly NormalizadorSinalService Instance = new(Grupo);
    }
}
