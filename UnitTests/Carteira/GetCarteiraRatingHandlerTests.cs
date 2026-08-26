using Application.Handlers.Carteira;
using Application.Queries.Carteira;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Interface;
using Moq;
using System.Linq.Expressions;

namespace UnitTests.Carteira
{
    public class GetCarteiraRatingHandlerTests
    {
        private readonly Mock<IBaseRepository<CapagCalculadoraResultado>> _capagRepoMock = new();
        private readonly Mock<IBaseRepository<Empresa>> _empresaRepoMock = new();

        private GetCarteiraRatingHandler BuildHandler() => new(
            _capagRepoMock.Object,
            _empresaRepoMock.Object);

        private void SetupCapag(IQueryable<CapagCalculadoraResultado> data)
        {
            _capagRepoMock
                .Setup(r => r.Query(It.IsAny<Expression<Func<CapagCalculadoraResultado, bool>>>(), It.IsAny<bool>()))
                .Returns(data);
        }

        private void SetupEmpresas(IQueryable<Empresa> data)
        {
            _empresaRepoMock
                .Setup(r => r.Query(It.IsAny<Expression<Func<Empresa, bool>>>(), It.IsAny<bool>()))
                .Returns(data);
        }

        [Fact]
        public async Task Handle_retorna_estrutura_vazia_quando_nao_ha_calculos()
        {
            SetupCapag(Enumerable.Empty<CapagCalculadoraResultado>().AsQueryable());
            SetupEmpresas(Enumerable.Empty<Empresa>().AsQueryable());

            var handler = BuildHandler();
            var result = await handler.Handle(new GetCarteiraRatingQuery(), CancellationToken.None);

            result.Meses.Should().BeEmpty();
            result.Totais.TotalAnalisado.Should().Be(0);
            result.Totais.ComImpedimento.Total.Should().Be(0);
            result.Totais.SemImpedimento.Total.Should().Be(0);
        }

        [Fact]
        public async Task Handle_agrupa_ratings_por_mes_em_sem_impedimento()
        {
            var ratings = new List<CapagCalculadoraResultado>
            {
                new() { IdEmpresa = 1, Parcial = false, Classificacao = "A", DateUpdate = new DateTime(2026, 2, 15) },
                new() { IdEmpresa = 2, Parcial = false, Classificacao = "B", DateUpdate = new DateTime(2026, 2, 20) },
                new() { IdEmpresa = 3, Parcial = false, Classificacao = "C", DateUpdate = new DateTime(2026, 3, 10) },
            }.AsQueryable();

            SetupCapag(ratings);
            SetupEmpresas(Enumerable.Empty<Empresa>().AsQueryable());

            var handler = BuildHandler();
            var result = await handler.Handle(new GetCarteiraRatingQuery
            {
                MesDe = "02/2026",
                MesAte = "03/2026"
            }, CancellationToken.None);

            result.Meses.Should().HaveCount(2);

            var fev = result.Meses.Single(r => r.Mes == "02/2026");
            fev.SemImpedimento.A.Should().Be(1);
            fev.SemImpedimento.B.Should().Be(1);
            fev.SemImpedimento.Total.Should().Be(2);
            fev.ComImpedimento.Total.Should().Be(0);
            fev.TotalAnalisado.Should().Be(2);

            var mar = result.Meses.Single(r => r.Mes == "03/2026");
            mar.SemImpedimento.C.Should().Be(1);
            mar.TotalAnalisado.Should().Be(1);

            result.Totais.SemImpedimento.A.Should().Be(1);
            result.Totais.SemImpedimento.B.Should().Be(1);
            result.Totais.SemImpedimento.C.Should().Be(1);
            result.Totais.SemImpedimento.Total.Should().Be(3);
            result.Totais.TotalAnalisado.Should().Be(3);
        }

        [Fact]
        public async Task Handle_empresa_impedida_mantem_letra_no_bucket_com_impedimento()
        {
            var ratings = new List<CapagCalculadoraResultado>
            {
                new() { IdEmpresa = 10, Parcial = false, Classificacao = "A", DateUpdate = new DateTime(2026, 4, 1) },
                new() { IdEmpresa = 11, Parcial = false, Classificacao = "B", DateUpdate = new DateTime(2026, 4, 2) }
            }.AsQueryable();

            var empresas = new List<Empresa>
            {
                new() { IdEmpresa = 10, DataImpedimento = new DateTime(2026, 3, 1) },
                new() { IdEmpresa = 11, DataImpedimento = null }
            }.AsQueryable();

            SetupCapag(ratings);
            SetupEmpresas(empresas);

            var handler = BuildHandler();
            var result = await handler.Handle(new GetCarteiraRatingQuery
            {
                MesDe = "04/2026",
                MesAte = "04/2026"
            }, CancellationToken.None);

            var abril = result.Meses.Single(r => r.Mes == "04/2026");
            abril.ComImpedimento.A.Should().Be(1);
            abril.ComImpedimento.Total.Should().Be(1);
            abril.SemImpedimento.B.Should().Be(1);
            abril.SemImpedimento.Total.Should().Be(1);
            abril.TotalAnalisado.Should().Be(2);

            result.Totais.ComImpedimento.A.Should().Be(1);
            result.Totais.SemImpedimento.B.Should().Be(1);
            result.Totais.TotalAnalisado.Should().Be(2);
        }

        [Fact]
        public async Task Handle_inclui_meses_zerados_no_intervalo_sem_dados()
        {
            var ratings = new List<CapagCalculadoraResultado>
            {
                new() { IdEmpresa = 1, Parcial = false, Classificacao = "A", DateUpdate = new DateTime(2026, 1, 1) },
                new() { IdEmpresa = 2, Parcial = false, Classificacao = "B", DateUpdate = new DateTime(2026, 3, 1) }
            }.AsQueryable();

            SetupCapag(ratings);
            SetupEmpresas(Enumerable.Empty<Empresa>().AsQueryable());

            var handler = BuildHandler();
            var result = await handler.Handle(new GetCarteiraRatingQuery
            {
                MesDe = "01/2026",
                MesAte = "03/2026"
            }, CancellationToken.None);

            result.Meses.Should().HaveCount(3);
            result.Meses.Single(r => r.Mes == "02/2026").TotalAnalisado.Should().Be(0);
            result.Totais.TotalAnalisado.Should().Be(2);
        }
    }
}
