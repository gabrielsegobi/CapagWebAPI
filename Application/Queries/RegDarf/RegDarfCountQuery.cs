using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.RegDarf
{
    public class RegDarfCountQuery : IRequest<GetApiResponse>
    {
        public RegDarfCountQuery(long idFilename)
        {
            IdFilename = idFilename;
        }

        public long IdFilename { get; set; }
    }
}
