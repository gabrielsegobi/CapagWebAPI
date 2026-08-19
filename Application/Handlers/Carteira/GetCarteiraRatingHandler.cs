using Application.Queries.Carteira;
using Domain.Contracts.Carteira;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.Carteira
{
    public class GetCarteiraRatingHandler : IRequestHandler<GetCarteiraRatingQuery, List<CarteiraRatingMesDto>>
    {
        private readonly IBaseRepository<CapagCalculadoraResultado> _capagRepository;
        private readonly IBaseRepository<Empresa> _empresaRepository;

        public GetCarteiraRatingHandler(
            IBaseRepository<CapagCalculadoraResultado> capagRepository,
            IBaseRepository<Empresa> empresaRepository)
        {
            _capagRepository = capagRepository;
            _empresaRepository = empresaRepository;
        }

        public async Task<List<CarteiraRatingMesDto>> Handle(
            GetCarteiraRatingQuery request,
            CancellationToken cancellationToken)
        {
            // Carrega o rating mais recente concluído por empresa
            var ratings = await _capagRepository
                .Query(r => !r.Parcial)
                .GroupBy(r => r.IdEmpresa)
                .Select(g => new
                {
                    IdEmpresa = g.Key,
                    Classificacao = g.OrderByDescending(r => r.DateUpdate).First().Classificacao,
                    DataCalculo = g.OrderByDescending(r => r.DateUpdate).First().DateUpdate
                })
                .ToListAsync(cancellationToken);

            if (ratings.Count == 0)
                return [];

            // Carrega DataImpedimento das empresas envolvidas
            var ids = ratings.Select(r => r.IdEmpresa).Distinct().ToList();
            var empresas = await _empresaRepository
                .Query(e => ids.Contains(e.IdEmpresa))
                .Select(e => new { e.IdEmpresa, e.DataImpedimento })
                .ToListAsync(cancellationToken);

            var impedimentoLookup = empresas.ToDictionary(e => e.IdEmpresa, e => e.DataImpedimento);

            // Determina intervalo de meses
            var datas = ratings.Select(r => new DateOnly(r.DataCalculo.Year, r.DataCalculo.Month, 1)).ToList();

            var minExistente = datas.Min();
            var maxExistente = datas.Max();

            var inicio = ParseMes(request.MesDe) ?? minExistente;
            var fim = ParseMes(request.MesAte) ?? maxExistente;

            if (inicio > fim) (inicio, fim) = (fim, inicio);

            // Agrupa ratings por mês
            var resultado = new List<CarteiraRatingMesDto>();
            var atual = inicio;

            while (atual <= fim)
            {
                var mesStr = atual.ToString("MM/yyyy");

                // Para cada empresa, pega o rating cujo mês de data_calculo é o mês corrente
                var doMes = ratings
                    .Where(r => r.DataCalculo.Year == atual.Year && r.DataCalculo.Month == atual.Month)
                    .ToList();

                int a = 0, b = 0, c = 0, d = 0, imp = 0;

                foreach (var r in doMes)
                {
                    impedimentoLookup.TryGetValue(r.IdEmpresa, out var dataImp);
                    if (dataImp.HasValue)
                        imp++;
                    else
                        switch (r.Classificacao.ToUpperInvariant())
                        {
                            case "A": a++; break;
                            case "B": b++; break;
                            case "C": c++; break;
                            case "D": d++; break;
                        }
                }

                resultado.Add(new CarteiraRatingMesDto
                {
                    Mes = mesStr,
                    A = a,
                    B = b,
                    C = c,
                    D = d,
                    Impedimento = imp,
                    TotalAnalisado = a + b + c + d + imp
                });

                atual = atual.AddMonths(1);
            }

            return resultado;
        }

        private static DateOnly? ParseMes(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
            if (DateOnly.TryParseExact(
                    $"01/{valor}",
                    "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out var data))
                return data;
            return null;
        }
    }
}
