using Application.Filters;
using Domain.Contracts.RegIrpf;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.RegsIrpf
{
    public class GetAllRegsIrpfQuery : IRequest<PagedApiResponse<RegIrpfDto>>
    {
        public RegIrpfFilter Filter { get; set; }
        public GetAllRegsIrpfQuery(RegIrpfFilter filter)
        {
            Filter = filter;
        }
    }
}
