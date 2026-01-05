using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.Views.DashboardEmpresaAnual
{
    public class GetDEAViewByIdQuery : IRequest<GetApiResponse>
    {
        public long Id { get; set; }
    }
}
