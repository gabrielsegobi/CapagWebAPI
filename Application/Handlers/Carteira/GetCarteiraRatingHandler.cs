using Application.Queries.Carteira;
using Domain.Contracts.Carteira;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.Carteira
{
    public class GetCarteiraRatingHandler : IRequestHandler<GetCarteiraRatingQuery, CarteiraRatingDto>
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

        public async Task<CarteiraRatingDto> Handle(
            GetCarteiraRatingQuery request,
            CancellationToken cancellationToken)
        {
            // Rating mais recente concluído por empresa (histórico permanece na tabela)
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
                return new CarteiraRatingDto();

            var ids = ratings.Select(r => r.IdEmpresa).Distinct().ToList();
            var empresas = await _empresaRepository
                .Query(e => ids.Contains(e.IdEmpresa))
                .Select(e => new { e.IdEmpresa, e.DataImpedimento })
                .ToListAsync(cancellationToken);

            var impedimentoLookup = empresas.ToDictionary(e => e.IdEmpresa, e => e.DataImpedimento);

            var datas = ratings.Select(r => new DateOnly(r.DataCalculo.Year, r.DataCalculo.Month, 1)).ToList();
            var inicio = ParseMes(request.MesDe) ?? datas.Min();
            var fim = ParseMes(request.MesAte) ?? datas.Max();

            if (inicio > fim) (inicio, fim) = (fim, inicio);

            var meses = new List<CarteiraRatingMesDto>();
            var atual = inicio;

            while (atual <= fim)
            {
                var doMes = ratings
                    .Where(r => r.DataCalculo.Year == atual.Year && r.DataCalculo.Month == atual.Month)
                    .ToList();

                var comImpedimento = CarteiraRatingContagemDto.Empty();
                var semImpedimento = CarteiraRatingContagemDto.Empty();

                foreach (var r in doMes)
                {
                    impedimentoLookup.TryGetValue(r.IdEmpresa, out var dataImp);
                    var bucket = dataImp.HasValue ? comImpedimento : semImpedimento;
                    bucket.Incrementar(r.Classificacao);
                }

                meses.Add(new CarteiraRatingMesDto
                {
                    Mes = atual.ToString("MM/yyyy"),
                    ComImpedimento = comImpedimento,
                    SemImpedimento = semImpedimento,
                    TotalAnalisado = comImpedimento.Total + semImpedimento.Total
                });

                atual = atual.AddMonths(1);
            }

            var totaisCom = CarteiraRatingContagemDto.Somar(meses.Select(m => m.ComImpedimento));
            var totaisSem = CarteiraRatingContagemDto.Somar(meses.Select(m => m.SemImpedimento));

            return new CarteiraRatingDto
            {
                Meses = meses,
                Totais = new CarteiraRatingTotaisDto
                {
                    ComImpedimento = totaisCom,
                    SemImpedimento = totaisSem,
                    TotalAnalisado = totaisCom.Total + totaisSem.Total
                }
            };
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
