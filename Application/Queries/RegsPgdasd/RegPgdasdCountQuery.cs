using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.RegsPgdasd
{
    public class RegPgdasdCountQuery : IRequest<GetApiResponse>
    {
        public RegPgdasdCountQuery(long idFilename)
        {
            IdFilename = idFilename;
        }

        public long IdFilename { get; set; }
    }
}
