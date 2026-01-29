using Application.Exceptions.Base;

namespace Application.Exceptions.SimulacoesIntervalo
{
    public class DuplicateIntervalException : ConflictException
    {
        public DuplicateIntervalException() : base("Já existe um intervalo do tipo ENTRADA para esta simulação.")
        {
        }
    }
}
