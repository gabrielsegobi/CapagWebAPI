using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Sped.Ecf
{
    public class EcfVersionLayout
    {
        /// <summary>
        /// Regras de ocorrência do registro dentro do arquivo.
        /// </summary>
        // public OcorrenciaLayout Ocorrencia { get; set; } = new();

        public List<FieldLayout> Campos { get; set; } = new();
    }
}
