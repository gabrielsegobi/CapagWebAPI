using Application.Exceptions.Base;

namespace Application.Exceptions.TipoGrupo
{
    public class TipoGrupoNotFoundException : NotFoundException
    {
        public TipoGrupoNotFoundException(long id) : base($"Grupo não encontrado para o ID: {id}")
        {
        }
    }
}
