using Application.Interfaces;
using Application.Services.Demonstrativos;
using Domain.Constants;
using Domain.Contracts.Capag;
using Domain.Contracts.Demonstrativos;

namespace Application.Services.Capag
{
    public class GreBuilderService
    {
        private readonly IDreCapagStructureProvider _dreStructureProvider;
        private readonly INormalizadorSinalService _normalizador;
        private readonly LucroBrutoCalculator _lucroBrutoCalculator;

        public GreBuilderService(
            IDreCapagStructureProvider dreStructureProvider,
            INormalizadorSinalService normalizador,
            LucroBrutoCalculator lucroBrutoCalculator)
        {
            _dreStructureProvider = dreStructureProvider;
            _normalizador = normalizador;
            _lucroBrutoCalculator = lucroBrutoCalculator;
        }

        public async Task<GreResultadoDto> ConstruirGre(long empresaId, CancellationToken cancellationToken = default)
        {
            var estrutura = await _dreStructureProvider.ObterEstruturaDreCapag(empresaId, cancellationToken);

            foreach (var conta in estrutura.ContasFolha)
            {
                var linha = DemonstrativoArvoreBuilder.Buscar(estrutura.Linhas, conta.Codigo);
                if (linha == null || linha.Excluida)
                    continue;

                linha.Valores.PorAno[conta.Ano] = _normalizador.Normalizar(
                    conta.Codigo,
                    conta.SaldoBruto,
                    conta.Indicador);
            }

            DemonstrativoArvoreBuilder.RecalcularSomaHierarquica(estrutura.Linhas, estrutura.Anos);
            _lucroBrutoCalculator.InserirNaArvore(estrutura.Linhas, estrutura.Anos);

            var lucroBruto = DemonstrativoArvoreBuilder.Buscar(estrutura.Linhas, DreCapagConstants.CodigoLucroBrutoCalculado);

            return new GreResultadoDto
            {
                IdEmpresa = empresaId,
                Anos = estrutura.Anos,
                Linhas = estrutura.Linhas,
                LucroBruto = lucroBruto
            };
        }
    }
}
