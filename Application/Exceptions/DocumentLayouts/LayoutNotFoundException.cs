using Application.Exceptions.Base;

namespace Application.Exceptions.DocumentLayouts
{
    public class LayoutNotFoundException : NotFoundException
    {
        public LayoutNotFoundException(long id) : base($"Layout com o ID: {id} não encontrado.")
        {
        }
    }
}
