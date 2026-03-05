//namespace Domain.Entities.Sped.Ecf
//{
//    /// <summary>
//    /// Representa um registro SPED completo. Ex: P100, L030, J005.
//    /// Responsabilidade: carregar metadados de um registro (código, tabela, hierarquia, campos).
//    /// Não faz nada além de transportar dados.
//    /// </summary>
//    public class EcfLayout
//    {
//        /// <summary>
//        /// Código do registro conforme manual SPED. Ex: "P100", "L030"
//        /// </summary>
//        public string Registro { get; set; } = string.Empty;

//        /// <summary>
//        /// Nome da tabela de destino no banco de dados. Ex: "SpedEcfP100"
//        /// </summary>
//        public string Tabela { get; set; } = string.Empty;

//        /// <summary>
//        /// Nível na hierarquia do arquivo SPED.
//        /// Usado pelo AjustarContexto() para resolver pai/filho.
//        /// Ex: 0000=nível 1, 0001=nível 2, P001=nível 3, P030=nível 4, P100=nível 5
//        /// </summary>
//        public int Nivel { get; set; }

//        /// <summary>
//        /// Código do registro pai conforme hierarquia SPED.
//        /// null = registro raiz (ex: 0000)
//        /// Ex: P100 tem Pai = "P030"
//        /// </summary>
//        public string? Pai { get; set; }

//        /// <summary>
//        /// Lista de campos do registro na ordem do layout JSON.
//        /// </summary>
//        public List<EcfVersionLayout> Versoes { get; set; } = new();
//    }
//}



namespace Domain.Entities.Sped.Ecf
{
    public class EcfLayout
    {
        public string Registro { get; set; } = string.Empty;
        public string Tabela { get; set; } = string.Empty;
        public int Nivel { get; set; }
        public string? Pai { get; set; }

        /// <summary>
        /// Mapa de layoutNumber → chave de versão. Ex: { "12": "v0" }
        /// </summary>
        public Dictionary<string, string> LayoutIndex { get; set; } = new();

        /// <summary>
        /// Mapa de chave de versão → dados da versão. Ex: { "v0": { ... } }
        /// </summary>
        public Dictionary<string, EcfVersionLayout> Versoes { get; set; } = new();

        /// <summary>
        /// Retorna os campos para um número de layout específico. O(1).
        /// </summary>
        public List<FieldLayout> GetCampos(int layout)
        {
            if (LayoutIndex.TryGetValue(layout.ToString(), out var chave) &&
                Versoes.TryGetValue(chave, out var versao))
                return versao.Campos;

            return new();
        }
    }
}