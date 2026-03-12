using Application.Exceptions.Base;
using System.Diagnostics.Eventing.Reader;

namespace Application.Exceptions.RegIrpf
{
    public class RegIrpfFileNotFoundException : NotFoundException
    {
        public RegIrpfFileNotFoundException(long id) : base($"Não foi encontrado registros irpf para o IdFilename: {id}")
        {
        }
    }
}
