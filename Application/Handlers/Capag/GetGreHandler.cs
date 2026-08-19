using Application.Exceptions.Empresas;
using Application.Queries.Capag;
using Application.Services.Capag;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Capag
{
    public class GetGreHandler : IRequestHandler<GetGreQuery, GetApiResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly GreBuilderService _greBuilder;

        public GetGreHandler(IBaseRepository<Empresa> empresaRepository, GreBuilderService greBuilder)
        {
            _empresaRepository = empresaRepository;
            _greBuilder = greBuilder;
        }

        public async Task<GetApiResponse> Handle(GetGreQuery request, CancellationToken cancellationToken)
        {
            var existe = await _empresaRepository.AnyAsync(e => e.IdEmpresa == request.EmpresaId && e.DeletedAt == null);
            if (!existe)
                throw new EmpresaNotFoundException(request.EmpresaId);

            var gre = await _greBuilder.ConstruirGre(request.EmpresaId, cancellationToken);
            return new GetApiResponse
            {
                Message = "GRE obtida com sucesso",
                Data = gre
            };
        }
    }
}
