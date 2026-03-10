using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.RegDefis
{
    public class RegDefisCountQuery : IRequest<GetApiResponse>
    {
        public RegDefisCountQuery(long idFilename)
        {
            IdFilename = idFilename;
        }

        public long IdFilename { get; set; }
    }
}
