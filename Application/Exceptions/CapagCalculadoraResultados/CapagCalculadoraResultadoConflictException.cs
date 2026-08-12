using Application.Exceptions.Base;

namespace Application.Exceptions.CapagCalculadoraResultados
{
    public class CapagCalculadoraResultadoConflictException : ConflictException
    {
        public CapagCalculadoraResultadoConflictException(long idEmpresa, string modelo)
            : base($"Já existe um resultado da calculadora CAPAG para a empresa {idEmpresa} e modelo '{modelo}'.")
        {
        }
    }
}
