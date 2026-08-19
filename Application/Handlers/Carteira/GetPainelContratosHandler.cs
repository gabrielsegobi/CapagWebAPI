using Application.Queries.Carteira;
using Domain.Contracts.Carteira;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.Carteira
{
    public class GetPainelContratosHandler : IRequestHandler<GetPainelContratosQuery, PainelContratosDto>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<CapagCalculadoraResultado> _capagRepository;

        public GetPainelContratosHandler(
            IBaseRepository<Empresa> empresaRepository,
            IBaseRepository<CapagCalculadoraResultado> capagRepository)
        {
            _empresaRepository = empresaRepository;
            _capagRepository = capagRepository;
        }

        public async Task<PainelContratosDto> Handle(GetPainelContratosQuery request, CancellationToken cancellationToken)
        {
            // IDs de empresas com ao menos um cálculo CAPAG concluído
            var idsComCapag = await _capagRepository
                .Query(r => !r.Parcial)
                .Select(r => r.IdEmpresa)
                .Distinct()
                .ToListAsync(cancellationToken);

            var empresas = await _empresaRepository.Query().ToListAsync(cancellationToken);

            // Calculo efetuado: tem CAPAG concluído, sem impedimento e status nulo ou 'calculo_efetuado'
            var comCalculo = empresas.Count(e =>
                idsComCapag.Contains(e.IdEmpresa) &&
                !e.DataImpedimento.HasValue &&
                (e.Status == null || e.Status == StatusComercialEmpresa.CalculoEfetuado));

            var emNegociacao = empresas
                .Where(e => e.Status == StatusComercialEmpresa.EmNegociacao)
                .ToList();

            var fechados = empresas
                .Where(e => e.Status == StatusComercialEmpresa.ContratoFechado)
                .ToList();

            return new PainelContratosDto
            {
                EmpresasComCalculoEfetuado = comCalculo,
                EmNegociacao = emNegociacao.Count,
                ValorEstimadoDasNegociacoes = emNegociacao.Sum(e => e.ValorContrato ?? 0m),
                ContratosFechados = fechados.Count,
                ValorArrecadadoContratosFechados = fechados.Sum(e => e.ValorContrato ?? 0m)
            };
        }
    }
}
