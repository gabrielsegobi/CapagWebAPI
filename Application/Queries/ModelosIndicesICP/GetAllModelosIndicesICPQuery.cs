using Application.Filters;
using Domain.Contracts.ModelosIndicesICP;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.ModelosIndicesICP
{
    public class GetAllModelosIndicesICPQuery: IRequest<PagedApiResponse<ModeloIndiceICPDto>>
    {

        public ModeloIndiceICPFilter Filter { get; set; }

        public GetAllModelosIndicesICPQuery(ModeloIndiceICPFilter filter)
        {
            Filter = filter;
        }

    }
}
