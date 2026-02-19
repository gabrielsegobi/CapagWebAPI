namespace Domain.Interfaces
{
    public interface IEcfFactory
    {
        IEcfBuilderStrategy ObterPorReg(string reg);
    }
}
  