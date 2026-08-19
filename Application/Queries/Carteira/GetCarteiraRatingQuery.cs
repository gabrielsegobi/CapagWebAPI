using Domain.Contracts.Carteira;
using MediatR;

namespace Application.Queries.Carteira
{
    public class GetCarteiraRatingQuery : IRequest<List<CarteiraRatingMesDto>>
    {
        /// <summary>Mês inicial no formato MM/yyyy (ex: 02/2026). Opcional.</summary>
        public string? MesDe { get; set; }

        /// <summary>Mês final no formato MM/yyyy (ex: 07/2026). Opcional.</summary>
        public string? MesAte { get; set; }
    }
}
