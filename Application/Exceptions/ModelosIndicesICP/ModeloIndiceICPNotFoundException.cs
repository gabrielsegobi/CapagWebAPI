using Application.Exceptions.Base;

namespace Application.Exceptions.ModelosIndicesICP
{
    public class ModeloIndiceICPNotFoundException : NotFoundException
    {
        public ModeloIndiceICPNotFoundException(long id) : base($"Modelo Com o ID:{id} não foi encontrado")
        {
        }
    }
}
