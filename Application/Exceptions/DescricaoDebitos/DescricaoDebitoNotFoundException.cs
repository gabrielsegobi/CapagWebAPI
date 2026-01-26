using Application.Exceptions.Base;

namespace Application.Exceptions.DescricaoDebitos
{
    public class DescricaoDebitoNotFoundException : NotFoundException
    {
        public DescricaoDebitoNotFoundException(long id) : base($"Descrição não encontrada para o ID: {id}")
        {
        }
    }
}
