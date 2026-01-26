using Application.Filters;
using Domain.Contracts.ICPAnterior;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.ICPAnterior
{
    public class GetAllICPAnteriorQuery : IRequest<PagedApiResponse<ICPAnteriorDto>>
    {
        public GetAllICPAnteriorQuery(ICPAnteriorFilter filter)
        {
            Filter = filter;
        }

        public ICPAnteriorFilter Filter { get; set; }
    }
}
