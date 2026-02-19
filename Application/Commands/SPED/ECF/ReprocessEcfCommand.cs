using MediatR;

namespace Application.Commands.SPED.ECF
{
    public class ReprocessEcfCommand : IRequest
    {
        public ReprocessEcfCommand(long idEmpresa)
        {
            IdEmpresa = idEmpresa;
        }

        public long IdEmpresa { get; set; }


    }
}
