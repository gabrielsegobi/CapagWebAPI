using Application.Exceptions.Empresas;
using Application.Queries.Demonstrativos;
using Application.Services.Demonstrativos;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Demonstrativos
{
    public class GetBalancoHandler : IRequestHandler<GetBalancoQuery, GetApiResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly DemonstrativoConsultaService _consulta;

        public GetBalancoHandler(
            IBaseRepository<Empresa> empresaRepository,
            DemonstrativoConsultaService consulta)
        {
            _empresaRepository = empresaRepository;
            _consulta = consulta;
        }

        public async Task<GetApiResponse> Handle(GetBalancoQuery request, CancellationToken cancellationToken)
        {
            var existe = await _empresaRepository.AnyAsync(e => e.IdEmpresa == request.EmpresaId && e.DeletedAt == null);
            if (!existe)
                throw new EmpresaNotFoundException(request.EmpresaId);

            var data = await _consulta.ObterBalanco(request.EmpresaId, cancellationToken);

            return new GetApiResponse
            {
                Message = "Balanço patrimonial obtido com sucesso",
                Data = data
            };
        }
    }
}
