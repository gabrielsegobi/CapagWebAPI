using Application.Exceptions.Base;

namespace Application.Exceptions.DocumentLayouts
{
    public class DocumentLayoutNotFoundException(long id) : NotFoundException($"Layout com o ID: {id} não encontrado.")
    {
    }
}
