using Application.Exceptions.Empresas;
using Application.Exceptions.TipoGrupo;
using Application.Factories;
using Application.Queries.TipoGrupos;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.TipoGrupos
{
    public class GetVariaveisHandler : IRequestHandler<GetVariaveisQuery, GetApiResponse>
    {
        private readonly IBaseRepository<TipoGrupo> _baseRepository;
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly ICalculoGrupoFactory _strategyFactory;

        public GetVariaveisHandler(IBaseRepository<TipoGrupo> baseRepository, IBaseRepository<Empresa> empresaRepository, ICalculoGrupoFactory factory)
        {
            _baseRepository = baseRepository;
            _empresaRepository = empresaRepository;
            _strategyFactory = factory;
        }

        public async Task<GetApiResponse> Handle(GetVariaveisQuery request, CancellationToken cancellationToken)
        {
            _ = await _empresaRepository.GetByIdAsync(request.Request.IdEmpresa) ?? throw new EmpresaNotFoundException(request.Request.IdEmpresa);

            var grupo = await _baseRepository.GetByIdAsync(request.Id) ?? throw new TipoGrupoNotFoundException(request.Id);

            var strategy = _strategyFactory.ObterPorTag(grupo.Tag);

            var resultado = await strategy.CalcularAsync(request.Request.IdEmpresa, request.Request.Ano, cancellationToken);

            return resultado;
        }
    }
}
