using Application.Exceptions.Base;

namespace Application.Exceptions.CapagCalculadoraResultados
{
    public class CapagCalculadoraResultadoNotFoundException : NotFoundException
    {
        public CapagCalculadoraResultadoNotFoundException(long id)
            : base($"Resultado da calculadora CAPAG não encontrado para o ID: {id}")
        {
        }
    }
}
