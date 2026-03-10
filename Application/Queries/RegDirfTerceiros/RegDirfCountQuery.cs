using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.RegDirfTerceiros
{
    public class RegDirfCountQuery : IRequest<GetApiResponse>
    {
        public RegDirfCountQuery(long idFilename)
        {
            IdFilename = idFilename;
        }

        public long IdFilename { get; set; }
    }
}
