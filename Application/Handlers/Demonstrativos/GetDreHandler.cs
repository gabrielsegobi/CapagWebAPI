using Application.Exceptions.Empresas;
using Application.Queries.Demonstrativos;
using Application.Services.Demonstrativos;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Demonstrativos
{
    public class GetDreHandler : IRequestHandler<GetDreQuery, GetApiResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly DemonstrativoConsultaService _consulta;

        public GetDreHandler(
            IBaseRepository<Empresa> empresaRepository,
            DemonstrativoConsultaService consulta)
        {
            _empresaRepository = empresaRepository;
            _consulta = consulta;
        }

        public async Task<GetApiResponse> Handle(GetDreQuery request, CancellationToken cancellationToken)
        {
            var existe = await _empresaRepository.AnyAsync(e => e.IdEmpresa == request.EmpresaId && e.DeletedAt == null);
            if (!existe)
                throw new EmpresaNotFoundException(request.EmpresaId);

            var data = await _consulta.ObterDre(request.EmpresaId, cancellationToken);

            return new GetApiResponse
            {
                Message = "DRE obtida com sucesso",
                Data = data
            };
        }
    }
}
