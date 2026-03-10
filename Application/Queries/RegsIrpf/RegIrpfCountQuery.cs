using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.RegsIrpf
{
    public class RegIrpfCountQuery : IRequest<GetApiResponse>
    {
        public RegIrpfCountQuery(long idFilename)
        {
            IdFilename = idFilename;
        }

        public long IdFilename { get; set; }
    }
}
