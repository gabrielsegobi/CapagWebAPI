using Application.Exceptions.Base;

namespace Application.Exceptions.RegIrpf
{
    public class RegIrpfNotFoundException : NotFoundException
    {
        public RegIrpfNotFoundException(long id, string ano) : base($"Valores não encontrados para a empresa com o ID: {id} ou ANO: {ano}")
        {
        }
    }
}
