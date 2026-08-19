using Application.Handlers.Carteira;
using Application.Queries.Carteira;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Interface;
using Moq;
using System.Linq.Expressions;

namespace UnitTests.Carteira
{
    public class GetPainelContratosHandlerTests
    {
        private readonly Mock<IBaseRepository<Empresa>> _empresaRepoMock = new();
        private readonly Mock<IBaseRepository<CapagCalculadoraResultado>> _capagRepoMock = new();

        private GetPainelContratosHandler BuildHandler() => new(
            _empresaRepoMock.Object,
            _capagRepoMock.Object);

        private void SetupEmpresas(IQueryable<Empresa> data)
        {
            _empresaRepoMock
                .Setup(r => r.Query(It.IsAny<Expression<Func<Empresa, bool>>>(), It.IsAny<bool>()))
                .Returns(data);
        }

        private void SetupCapag(IQueryable<CapagCalculadoraResultado> data)
        {
            _capagRepoMock
                .Setup(r => r.Query(It.IsAny<Expression<Func<CapagCalculadoraResultado, bool>>>(), It.IsAny<bool>()))
                .Returns(data);
        }

        [Fact]
        public async Task Handle_conta_empresas_com_calculo_efetuado_corretamente()
        {
            var empresas = new List<Empresa>
            {
                // Tem CAPAG, sem impedimento, status nulo → calculo_efetuado
                new() { IdEmpresa = 1, Status = null },
                // Tem CAPAG, status explícito calculo_efetuado
                new() { IdEmpresa = 2, Status = StatusComercialEmpresa.CalculoEfetuado },
                // Tem CAPAG mas está impedida → não conta
                new() { IdEmpresa = 3, Status = null, DataImpedimento = DateTime.Today },
                // Tem CAPAG mas está em negociação → não conta aqui
                new() { IdEmpresa = 4, Status = StatusComercialEmpresa.EmNegociacao },
                // Sem CAPAG → não conta
                new() { IdEmpresa = 5, Status = null }
            }.AsQueryable();

            var ratings = new List<CapagCalculadoraResultado>
            {
                new() { IdEmpresa = 1, Parcial = false },
                new() { IdEmpresa = 2, Parcial = false },
                new() { IdEmpresa = 3, Parcial = false },
                new() { IdEmpresa = 4, Parcial = false }
            }.AsQueryable();

            SetupEmpresas(empresas);
            SetupCapag(ratings);

            var handler = BuildHandler();
            var result = await handler.Handle(new GetPainelContratosQuery(), CancellationToken.None);

            result.EmpresasComCalculoEfetuado.Should().Be(2); // ids 1 e 2
        }

        [Fact]
        public async Task Handle_soma_valor_negociacoes_corretamente()
        {
            var empresas = new List<Empresa>
            {
                new() { IdEmpresa = 1, Status = StatusComercialEmpresa.EmNegociacao, ValorContrato = 100_000m },
                new() { IdEmpresa = 2, Status = StatusComercialEmpresa.EmNegociacao, ValorContrato = 200_000m },
                new() { IdEmpresa = 3, Status = StatusComercialEmpresa.ContratoFechado, ValorContrato = 50_000m }
            }.AsQueryable();

            SetupEmpresas(empresas);
            SetupCapag(Enumerable.Empty<CapagCalculadoraResultado>().AsQueryable());

            var handler = BuildHandler();
            var result = await handler.Handle(new GetPainelContratosQuery(), CancellationToken.None);

            result.EmNegociacao.Should().Be(2);
            result.ValorEstimadoDasNegociacoes.Should().Be(300_000m);
        }

        [Fact]
        public async Task Handle_soma_contratos_fechados_corretamente()
        {
            var empresas = new List<Empresa>
            {
                new() { IdEmpresa = 1, Status = StatusComercialEmpresa.ContratoFechado, ValorContrato = 1_000_000m },
                new() { IdEmpresa = 2, Status = StatusComercialEmpresa.ContratoFechado, ValorContrato = null },
                new() { IdEmpresa = 3, Status = StatusComercialEmpresa.EmNegociacao, ValorContrato = 500_000m }
            }.AsQueryable();

            SetupEmpresas(empresas);
            SetupCapag(Enumerable.Empty<CapagCalculadoraResultado>().AsQueryable());

            var handler = BuildHandler();
            var result = await handler.Handle(new GetPainelContratosQuery(), CancellationToken.None);

            result.ContratosFechados.Should().Be(2);
            result.ValorArrecadadoContratosFechados.Should().Be(1_000_000m);
        }

        [Fact]
        public async Task Handle_retorna_zeros_quando_sem_dados()
        {
            SetupEmpresas(Enumerable.Empty<Empresa>().AsQueryable());
            SetupCapag(Enumerable.Empty<CapagCalculadoraResultado>().AsQueryable());

            var handler = BuildHandler();
            var result = await handler.Handle(new GetPainelContratosQuery(), CancellationToken.None);

            result.EmpresasComCalculoEfetuado.Should().Be(0);
            result.EmNegociacao.Should().Be(0);
            result.ValorEstimadoDasNegociacoes.Should().Be(0m);
            result.ContratosFechados.Should().Be(0);
            result.ValorArrecadadoContratosFechados.Should().Be(0m);
        }
    }
}
