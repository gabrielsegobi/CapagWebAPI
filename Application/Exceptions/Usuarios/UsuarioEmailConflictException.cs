using Application.Exceptions.Base;

namespace Application.Exceptions.Usuarios
{
    public class UsuarioEmailConflictException(string email) : ConflictException($"Já existe um usuário cadastrado com esse email: {email}")
    {
    }
}
