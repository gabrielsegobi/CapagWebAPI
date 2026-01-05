using Application.Exceptions.Base;

namespace Application.Exceptions.Usuarios
{
    public class UsuarioUnauthorizedException() : UnauthorizedException("Usuário ou senha inválidos")
    {
    }
}
