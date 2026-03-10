using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.RegsDctf
{
    public class RegDcftCountQuery : IRequest<GetApiResponse>
    {
        public RegDcftCountQuery(long idFilename)
        {
            IdFilename = idFilename;
        }

        public long IdFilename { get; set; }
    }
}
