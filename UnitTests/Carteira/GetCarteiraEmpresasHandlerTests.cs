using Application.Filters;
using Application.Handlers.Carteira;
using Application.Queries.Carteira;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Interface;
using Moq;
using System.Linq.Expressions;

namespace UnitTests.Carteira
{
    public class GetCarteiraEmpresasHandlerTests
    {
        private readonly Mock<IBaseRepository<Empresa>> _empresaRepoMock = new();
        private readonly Mock<IBaseRepository<CapagCalculadoraResultado>> _capagRepoMock = new();
        private readonly Mock<IBaseRepository<DemonstrativoContabil>> _demonstrativoRepoMock = new();
        private readonly Mock<IBaseRepository<Usuario>> _usuarioRepoMock = new();

        private GetCarteiraEmpresasHandler BuildHandler() => new(
            _empresaRepoMock.Object,
            _capagRepoMock.Object,
            _demonstrativoRepoMock.Object,
            _usuarioRepoMock.Object);

        private static void SetupCapag(Mock<IBaseRepository<CapagCalculadoraResultado>> mock,
            IQueryable<CapagCalculadoraResultado> data)
        {
            mock.Setup(r => r.Query(It.IsAny<Expression<Func<CapagCalculadoraResultado, bool>>>(), It.IsAny<bool>()))
                .Returns(data);
        }

        private static void SetupDemonstrativos(Mock<IBaseRepository<DemonstrativoContabil>> mock,
            IQueryable<DemonstrativoContabil> data)
        {
            mock.Setup(r => r.Query(It.IsAny<Expression<Func<DemonstrativoContabil, bool>>>(), It.IsAny<bool>()))
                .Returns(data);
        }

        private static void SetupUsuarios(Mock<IBaseRepository<Usuario>> mock, IQueryable<Usuario> data)
        {
            mock.Setup(r => r.Query(It.IsAny<Expression<Func<Usuario, bool>>>(), It.IsAny<bool>()))
                .Returns(data);
        }

        private static void SetupEmpresas(Mock<IBaseRepository<Empresa>> mock, IQueryable<Empresa> data)
        {
            mock.Setup(r => r.Query(It.IsAny<Expression<Func<Empresa, bool>>>(), It.IsAny<bool>()))
                .Returns(data);
        }

        [Fact]
        public async Task Handle_retorna_lista_paginada_com_status_bloqueio_calculado()
        {
            var empresas = new List<Empresa>
            {
                new() { IdEmpresa = 1, RazaoSocial = "Alpha", Cnpj = "00000000000001", DataImpedimento = null },
                new() { IdEmpresa = 2, RazaoSocial = "Beta",  Cnpj = "00000000000002", DataImpedimento = DateTime.Today }
            }.AsQueryable();

            SetupEmpresas(_empresaRepoMock, empresas);
            SetupCapag(_capagRepoMock, Enumerable.Empty<CapagCalculadoraResultado>().AsQueryable());
            SetupDemonstrativos(_demonstrativoRepoMock, Enumerable.Empty<DemonstrativoContabil>().AsQueryable());
            SetupUsuarios(_usuarioRepoMock, Enumerable.Empty<Usuario>().AsQueryable());

            var handler = BuildHandler();
            var result = await handler.Handle(new GetCarteiraEmpresasQuery(new CarteiraEmpresaFilter()), CancellationToken.None);

            result.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.Data.First(d => d.IdEmpresa == 1).StatusBloqueio.Should().Be("liberado");
            result.Data.First(d => d.IdEmpresa == 2).StatusBloqueio.Should().Be("bloqueado");
        }

        [Fact]
        public async Task Handle_filtro_data_impedimento_ate_traz_apenas_empresas_no_periodo()
        {
            var empresas = new List<Empresa>
            {
                new() { IdEmpresa = 1, RazaoSocial = "Antes", Cnpj = "00000000000001", DataImpedimento = new DateTime(2026, 5, 1) },
                new() { IdEmpresa = 2, RazaoSocial = "Depois", Cnpj = "00000000000002", DataImpedimento = new DateTime(2026, 8, 1) },
                new() { IdEmpresa = 3, RazaoSocial = "Sem",    Cnpj = "00000000000003", DataImpedimento = null }
            }.AsQueryable();

            SetupEmpresas(_empresaRepoMock, empresas);
            SetupCapag(_capagRepoMock, Enumerable.Empty<CapagCalculadoraResultado>().AsQueryable());
            SetupDemonstrativos(_demonstrativoRepoMock, Enumerable.Empty<DemonstrativoContabil>().AsQueryable());
            SetupUsuarios(_usuarioRepoMock, Enumerable.Empty<Usuario>().AsQueryable());

            var filter = new CarteiraEmpresaFilter { DataImpedimento = new DateTime(2026, 6, 30) };
            var handler = BuildHandler();
            var result = await handler.Handle(new GetCarteiraEmpresasQuery(filter), CancellationToken.None);

            // Apenas empresa 1 tem impedimento até 30/06/2026
            result.Data.Should().ContainSingle(d => d.IdEmpresa == 1);
        }

        [Fact]
        public async Task Handle_inclui_rating_capag_do_calculo_mais_recente_concluido()
        {
            var empresa = new Empresa { IdEmpresa = 10, RazaoSocial = "Corp", Cnpj = "11111111111111" };

            SetupEmpresas(_empresaRepoMock, new[] { empresa }.AsQueryable());

            var ratings = new List<CapagCalculadoraResultado>
            {
                new() { IdEmpresa = 10, Parcial = false, Classificacao = "A", DateUpdate = new DateTime(2026, 1, 1) },
                new() { IdEmpresa = 10, Parcial = false, Classificacao = "B", DateUpdate = new DateTime(2026, 6, 1) }
            }.AsQueryable();

            SetupCapag(_capagRepoMock, ratings);
            SetupDemonstrativos(_demonstrativoRepoMock, Enumerable.Empty<DemonstrativoContabil>().AsQueryable());
            SetupUsuarios(_usuarioRepoMock, Enumerable.Empty<Usuario>().AsQueryable());

            var handler = BuildHandler();
            var result = await handler.Handle(new GetCarteiraEmpresasQuery(new CarteiraEmpresaFilter()), CancellationToken.None);

            result.Data.Single().RatingCapag.Should().Be("B"); // mais recente
        }

        [Fact]
        public async Task Handle_filtro_statusBloqueio_liberado_exclui_empresas_bloqueadas()
        {
            var empresas = new List<Empresa>
            {
                new() { IdEmpresa = 1, RazaoSocial = "Livre",     Cnpj = "00000000000001" },
                new() { IdEmpresa = 2, RazaoSocial = "Bloqueada", Cnpj = "00000000000002", DataImpedimento = DateTime.Today }
            }.AsQueryable();

            SetupEmpresas(_empresaRepoMock, empresas);
            SetupCapag(_capagRepoMock, Enumerable.Empty<CapagCalculadoraResultado>().AsQueryable());
            SetupDemonstrativos(_demonstrativoRepoMock, Enumerable.Empty<DemonstrativoContabil>().AsQueryable());
            SetupUsuarios(_usuarioRepoMock, Enumerable.Empty<Usuario>().AsQueryable());

            var filter = new CarteiraEmpresaFilter { StatusBloqueio = "liberado" };
            var handler = BuildHandler();
            var result = await handler.Handle(new GetCarteiraEmpresasQuery(filter), CancellationToken.None);

            result.Data.Should().ContainSingle(d => d.IdEmpresa == 1);
        }
    }
}
