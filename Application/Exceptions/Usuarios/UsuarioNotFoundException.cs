using Application.Exceptions.Base;

namespace Application.Exceptions.Usuarios
{
    public class UsuarioNotFoundException : NotFoundException
    {
        public UsuarioNotFoundException(long id)
           : base($"Usuário com o ID:{id} não foi encontrado.")
        {
        }
    }
}
