using Application.Exceptions.Empresas;
using Application.Queries.PrlA;
using Application.Services.PrlA;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.PrlA
{
    public class GetPrlAHandler : IRequestHandler<GetPrlAQuery, GetApiResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IPrlAService _prlAService;

        public GetPrlAHandler(IBaseRepository<Empresa> empresaRepository, IPrlAService prlAService)
        {
            _empresaRepository = empresaRepository;
            _prlAService = prlAService;
        }

        public async Task<GetApiResponse> Handle(GetPrlAQuery request, CancellationToken cancellationToken)
        {
            var existe = await _empresaRepository.AnyAsync(e => e.IdEmpresa == request.EmpresaId && e.DeletedAt == null);
            if (!existe)
                throw new EmpresaNotFoundException(request.EmpresaId);

            var resultado = await _prlAService.ObterAsync(request.EmpresaId, request.Ano, cancellationToken);
            return new GetApiResponse
            {
                Message = "PRL-A obtido com sucesso",
                Data = resultado
            };
        }
    }
}
