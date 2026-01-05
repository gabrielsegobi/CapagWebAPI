using Application.Exceptions.Base;

namespace Application.Exceptions.Empresas
{
    public class EmpresaNotFoundException : NotFoundException
    {
        public EmpresaNotFoundException(long id)
            : base($"Empresa com o ID:{id} não foi encontrada.")
        {
        }
    }
}
