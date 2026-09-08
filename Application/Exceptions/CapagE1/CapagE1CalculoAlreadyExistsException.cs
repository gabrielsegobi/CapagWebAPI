using Application.Exceptions.Base;

namespace Application.Exceptions.CapagE1
{
    public class CapagE1CalculoAlreadyExistsException : ConflictException
    {
        public CapagE1CalculoAlreadyExistsException(long idEmpresa, string modelo)
            : base($"Já existe cálculo CAPAG-e1 para a empresa {idEmpresa} (modelo {modelo}). Use PUT para atualizar.")
        {
        }
    }
}
