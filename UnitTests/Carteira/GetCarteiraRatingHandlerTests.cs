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
        public async Task Handle_retorna_lista_vazia_quando_nao_ha_calculos()
        {
            SetupCapag(Enumerable.Empty<CapagCalculadoraResultado>().AsQueryable());
            SetupEmpresas(Enumerable.Empty<Empresa>().AsQueryable());

            var handler = BuildHandler();
            var result = await handler.Handle(new GetCarteiraRatingQuery(), CancellationToken.None);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_agrupa_ratings_por_mes_corretamente()
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

            result.Should().HaveCount(2);

            var fev = result.Single(r => r.Mes == "02/2026");
            fev.A.Should().Be(1);
            fev.B.Should().Be(1);
            fev.TotalAnalisado.Should().Be(2);

            var mar = result.Single(r => r.Mes == "03/2026");
            mar.C.Should().Be(1);
            mar.TotalAnalisado.Should().Be(1);
        }

        [Fact]
        public async Task Handle_conta_empresa_impedida_em_bucket_impedimento_nao_em_letra()
        {
            var ratings = new List<CapagCalculadoraResultado>
            {
                new() { IdEmpresa = 10, Parcial = false, Classificacao = "A", DateUpdate = new DateTime(2026, 4, 1) }
            }.AsQueryable();

            var empresas = new List<Empresa>
            {
                new() { IdEmpresa = 10, DataImpedimento = new DateTime(2026, 3, 1) }
            }.AsQueryable();

            SetupCapag(ratings);
            SetupEmpresas(empresas);

            var handler = BuildHandler();
            var result = await handler.Handle(new GetCarteiraRatingQuery
            {
                MesDe = "04/2026",
                MesAte = "04/2026"
            }, CancellationToken.None);

            var abril = result.Single(r => r.Mes == "04/2026");
            abril.Impedimento.Should().Be(1);
            abril.A.Should().Be(0);
            abril.TotalAnalisado.Should().Be(1);
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

            result.Should().HaveCount(3);
            result.Single(r => r.Mes == "02/2026").TotalAnalisado.Should().Be(0);
        }
    }
}
