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

        private static void SetupQuery<T>(Mock<IBaseRepository<T>> mock, IQueryable<T> data) where T : class
        {
            mock.Setup(r => r.Query(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<bool>()))
                .Returns((Expression<Func<T, bool>>? pred, bool _) =>
                    pred == null ? data : data.Where(pred));
        }

        private void SetupDefaults(
            IQueryable<Empresa> empresas,
            IQueryable<CapagCalculadoraResultado>? ratings = null,
            IQueryable<DemonstrativoContabil>? demonstrativos = null,
            IQueryable<Usuario>? usuarios = null)
        {
            SetupQuery(_empresaRepoMock, empresas);
            SetupQuery(_capagRepoMock, ratings ?? Enumerable.Empty<CapagCalculadoraResultado>().AsQueryable());
            SetupQuery(_demonstrativoRepoMock, demonstrativos ?? Enumerable.Empty<DemonstrativoContabil>().AsQueryable());
            SetupQuery(_usuarioRepoMock, usuarios ?? Enumerable.Empty<Usuario>().AsQueryable());
        }

        [Fact]
        public async Task Handle_retorna_lista_paginada_com_status_bloqueio_calculado()
        {
            var empresas = new List<Empresa>
            {
                new() { IdEmpresa = 1, RazaoSocial = "Alpha", Cnpj = "00000000000001", DataImpedimento = null },
                new() { IdEmpresa = 2, RazaoSocial = "Beta",  Cnpj = "00000000000002", DataImpedimento = DateTime.Today }
            }.AsQueryable();

            SetupDefaults(empresas);

            var handler = BuildHandler();
            var result = await handler.Handle(new GetCarteiraEmpresasQuery(new CarteiraEmpresaFilter()), CancellationToken.None);

            result.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.Paging.Total.Should().Be(2);
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

            SetupDefaults(empresas);

            var filter = new CarteiraEmpresaFilter { DataImpedimentoAte = new DateTime(2026, 6, 30) };
            var handler = BuildHandler();
            var result = await handler.Handle(new GetCarteiraEmpresasQuery(filter), CancellationToken.None);

            result.Data.Should().ContainSingle(d => d.IdEmpresa == 1);
            result.Paging.Total.Should().Be(1);
        }

        [Fact]
        public async Task Handle_filtro_data_impedimento_exata_traz_apenas_o_dia_informado()
        {
            var empresas = new List<Empresa>
            {
                new() { IdEmpresa = 1, RazaoSocial = "Antes",  Cnpj = "00000000000001", DataImpedimento = new DateTime(2026, 6, 29, 23, 59, 0) },
                new() { IdEmpresa = 2, RazaoSocial = "NoDia",  Cnpj = "00000000000002", DataImpedimento = new DateTime(2026, 6, 30, 14, 30, 0) },
                new() { IdEmpresa = 3, RazaoSocial = "Depois", Cnpj = "00000000000003", DataImpedimento = new DateTime(2026, 7, 1, 0, 0, 0) }
            }.AsQueryable();

            SetupDefaults(empresas);

            var filter = new CarteiraEmpresaFilter { DataImpedimento = new DateTime(2026, 6, 30) };
            var handler = BuildHandler();
            var result = await handler.Handle(new GetCarteiraEmpresasQuery(filter), CancellationToken.None);

            result.Data.Should().ContainSingle(d => d.IdEmpresa == 2);
            result.Paging.Total.Should().Be(1);
        }

        [Fact]
        public async Task Handle_inclui_rating_capag_do_calculo_mais_recente_concluido()
        {
            var empresa = new Empresa { IdEmpresa = 10, RazaoSocial = "Corp", Cnpj = "11111111111111" };

            var ratings = new List<CapagCalculadoraResultado>
            {
                new() { IdEmpresa = 10, Parcial = false, Classificacao = "A", DateUpdate = new DateTime(2026, 1, 1) },
                new() { IdEmpresa = 10, Parcial = false, Classificacao = "B", DateUpdate = new DateTime(2026, 6, 1) }
            }.AsQueryable();

            SetupDefaults(new[] { empresa }.AsQueryable(), ratings);

            var handler = BuildHandler();
            var result = await handler.Handle(new GetCarteiraEmpresasQuery(new CarteiraEmpresaFilter()), CancellationToken.None);

            result.Data.Single().RatingCapag.Should().Be("B");
        }

        [Fact]
        public async Task Handle_filtro_statusBloqueio_liberado_exclui_empresas_bloqueadas()
        {
            var empresas = new List<Empresa>
            {
                new() { IdEmpresa = 1, RazaoSocial = "Livre",     Cnpj = "00000000000001" },
                new() { IdEmpresa = 2, RazaoSocial = "Bloqueada", Cnpj = "00000000000002", DataImpedimento = DateTime.Today }
            }.AsQueryable();

            SetupDefaults(empresas);

            var filter = new CarteiraEmpresaFilter { StatusBloqueio = "liberado" };
            var handler = BuildHandler();
            var result = await handler.Handle(new GetCarteiraEmpresasQuery(filter), CancellationToken.None);

            result.Data.Should().ContainSingle(d => d.IdEmpresa == 1);
            result.Paging.Total.Should().Be(1);
        }

        [Fact]
        public async Task Handle_filtro_rating_pagina_sobre_o_conjunto_filtrado()
        {
            var empresas = Enumerable.Range(1, 15)
                .Select(i => new Empresa
                {
                    IdEmpresa = i,
                    RazaoSocial = $"Empresa {i:00}",
                    Cnpj = i.ToString("00000000000000")
                })
                .AsQueryable();

            var ratings = new List<CapagCalculadoraResultado>
            {
                new() { IdEmpresa = 1,  Parcial = false, Classificacao = "A", DateUpdate = new DateTime(2026, 1, 1) },
                new() { IdEmpresa = 12, Parcial = false, Classificacao = "a", DateUpdate = new DateTime(2026, 2, 1) },
                new() { IdEmpresa = 13, Parcial = false, Classificacao = "B", DateUpdate = new DateTime(2026, 3, 1) }
            }.AsQueryable();

            SetupDefaults(empresas, ratings);

            var filter = new CarteiraEmpresaFilter { RatingCapag = "a", Page = 1, PageSize = 10 };
            var handler = BuildHandler();
            var result = await handler.Handle(new GetCarteiraEmpresasQuery(filter), CancellationToken.None);

            result.Paging.Total.Should().Be(2);
            result.Data.Should().HaveCount(2);
            result.Data.Select(d => d.IdEmpresa).Should().BeEquivalentTo([1L, 12L]);
        }

        [Fact]
        public async Task Handle_exclui_empresas_com_deleted_at()
        {
            var empresas = new List<Empresa>
            {
                new() { IdEmpresa = 1, RazaoSocial = "Ativa", Cnpj = "00000000000001" },
                new() { IdEmpresa = 2, RazaoSocial = "Excluida", Cnpj = "00000000000002", DeletedAt = DateTime.UtcNow }
            }.AsQueryable();

            SetupDefaults(empresas);

            var handler = BuildHandler();
            var result = await handler.Handle(new GetCarteiraEmpresasQuery(new CarteiraEmpresaFilter()), CancellationToken.None);

            result.Data.Should().ContainSingle(d => d.IdEmpresa == 1);
            result.Paging.Total.Should().Be(1);
        }
    }
}
