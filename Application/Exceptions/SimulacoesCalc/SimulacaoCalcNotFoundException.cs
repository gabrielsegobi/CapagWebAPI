using Application.Exceptions.Base;

namespace Application.Exceptions.SimulacoesCalc
{
    public class SimulacaoCalcNotFoundException : NotFoundException
    {
        public SimulacaoCalcNotFoundException(long id) : base($"Simulação não encontrada para o ID: {id}")
        {
        }
    }
}
