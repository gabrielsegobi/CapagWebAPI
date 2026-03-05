namespace Domain.Entities.Sped
{
    public class SpedLayoutSettings
    {
        public const string Section = "SpedLayouts";

        /// <summary>
        /// Caminho relativo à raiz do projeto para os layouts ECF.
        /// Ex: "Resources/Sped/Ecf"
        /// Em runtime é combinado com AppContext.BaseDirectory.
        /// </summary>
        public string Ecf { get; set; } = "Resources/Sped/Ecf";
    }
}
