using Application.Exceptions.Base;

namespace Application.Exceptions.SimulacoesIntervalo
{
    public class SimulacaoIntervaloNotFoundException : NotFoundException
    {
        public SimulacaoIntervaloNotFoundException(long Id) : base($"Intervalo com o ID: {Id} não encontrado ")
        {
        }
    }
}
