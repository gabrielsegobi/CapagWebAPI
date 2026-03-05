using Domain.Entities.Sped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    /// <summary>
    /// Representa um registro SPED completo. Ex: P100, L030, J005.
    /// Responsabilidade: carregar metadados de um registro (código, tabela, hierarquia, campos).
    /// Não faz nada além de transportar dados.
    /// </summary>
    public class RegistroLayout
    {
        /// <summary>
        /// Código do registro conforme manual SPED. Ex: "P100", "L030"
        /// </summary>
        public string Registro { get; set; } = string.Empty;

        /// <summary>
        /// Nome da tabela de destino no banco de dados. Ex: "SpedEcfP100"
        /// </summary>
        public string Tabela { get; set; } = string.Empty;

        /// <summary>
        /// Nível na hierarquia do arquivo SPED.
        /// Usado pelo AjustarContexto() para resolver pai/filho.
        /// Ex: 0000=nível 1, 0001=nível 2, P001=nível 3, P030=nível 4, P100=nível 5
        /// </summary>
        public int Nivel { get; set; }

        /// <summary>
        /// Código do registro pai conforme hierarquia SPED.
        /// null = registro raiz (ex: 0000)
        /// Ex: P100 tem Pai = "P030"
        /// </summary>
        public string? Pai { get; set; }

        /// <summary>
        /// Lista de campos do registro na ordem do layout JSON.
        /// </summary>
        public List<FieldLayout> Campos { get; set; } = new();
    }
}
