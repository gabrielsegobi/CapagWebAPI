using Application.Exceptions.Base;

namespace Application.Exceptions.CapagE1
{
    public class CapagE1CalculoNotFoundException : NotFoundException
    {
        public CapagE1CalculoNotFoundException(long id)
            : base($"Cálculo CAPAG-e1 não encontrado para o ID: {id}")
        {
        }

        public CapagE1CalculoNotFoundException(long idEmpresa, string modelo)
            : base($"Cálculo CAPAG-e1 não encontrado para a empresa {idEmpresa} (modelo {modelo}).")
        {
        }
    }
}
