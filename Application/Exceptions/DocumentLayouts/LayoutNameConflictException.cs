using Application.Exceptions.Base;

namespace Application.Exceptions.DocumentLayouts
{
    public class LayoutNameConflictException : ConflictException
    {
        public LayoutNameConflictException(string name) : base($"Já existe um layout com o nome: {name}.")
        {
        }
    }
}
