using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.RegFileNames
{
    public class GetAllFilesByEmpresaQuery : IRequest<GetApiResponse>
    {
        public GetAllFilesByEmpresaQuery(long idEmpresa)
        {
            IdEmpresa = idEmpresa;
        }

        public long IdEmpresa { get; set; }
    }
}
