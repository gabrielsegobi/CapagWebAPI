using Application.Exceptions.Base;

namespace Application.Exceptions.Usuarios
{
    public class SenhaIncorretaUnauthorizedException : UnauthorizedException
    {
        public SenhaIncorretaUnauthorizedException() : base("Senha atual incorreta.")
        {
        }
    }
}
