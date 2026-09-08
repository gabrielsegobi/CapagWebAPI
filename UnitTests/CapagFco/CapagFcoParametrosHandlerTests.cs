using System.Linq.Expressions;
using System.Text.Json.Nodes;
using Application.Commands.CapagFco;
using Application.Exceptions.Empresas;
using Application.Handlers.CapagFco;
using Application.Helpers;
using Application.Queries.CapagFco;
using Domain.Contracts.CapagFco;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Interface;
using Moq;

namespace UnitTests.CapagFco
{
    public class CapagFcoParametrosHandlerTests
    {
        private readonly Mock<IBaseRepository<Empresa>> _empresaRepo = new();
        private readonly Mock<IBaseRepository<CapagFcoParametroEmpresa>> _parametroRepo = new();
        private readonly Mock<ICurrentUserService> _currentUser = new();

        public CapagFcoParametrosHandlerTests()
        {
            _currentUser.Setup(c => c.TenantId).Returns(10);
            _currentUser.Setup(c => c.UserId).Returns(99);
        }

        private GetCapagFcoParametrosHandler GetHandler() =>
            new(_empresaRepo.Object, _parametroRepo.Object, _currentUser.Object);

        private UpsertCapagFcoParametrosHandler UpsertHandler() =>
            new(_empresaRepo.Object, _parametroRepo.Object, _currentUser.Object);

        private void SetupEmpresa(long idEmpresa = 123, long idTenant = 10)
        {
            _empresaRepo
                .Setup(r => r.GetByIdAsync(idEmpresa))
                .ReturnsAsync(new Empresa { IdEmpresa = idEmpresa, IdTenant = idTenant });
        }

        [Fact]
        public async Task Get_sem_registro_retorna_excecoes_vazias()
        {
            SetupEmpresa();
            _parametroRepo
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CapagFcoParametroEmpresa, bool>>>()))
                .ReturnsAsync((CapagFcoParametroEmpresa?)null);

            var result = await GetHandler().Handle(
                new GetCapagFcoParametrosQuery { IdEmpresa = 123 },
                CancellationToken.None);

            result.CompanyId.Should().Be(123);
            result.ExcecoesL100.Count.Should().Be(0);
            result.ExcecoesL300.Count.Should().Be(0);
            result.ContasPersonalizadasL100.Should().Be(0);
        }

        [Fact]
        public async Task Get_empresa_inexistente_lanca_not_found()
        {
            _empresaRepo
                .Setup(r => r.GetByIdAsync(123))
                .ReturnsAsync((Empresa?)null);

            var act = () => GetHandler().Handle(
                new GetCapagFcoParametrosQuery { IdEmpresa = 123 },
                CancellationToken.None);

            await act.Should().ThrowAsync<EmpresaNotFoundException>();
        }

        [Fact]
        public async Task Put_cria_registro_so_com_o_bloco_enviado()
        {
            SetupEmpresa();
            _parametroRepo
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CapagFcoParametroEmpresa, bool>>>()))
                .ReturnsAsync((CapagFcoParametroEmpresa?)null);
            _parametroRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            CapagFcoParametroEmpresa? saved = null;
            _parametroRepo
                .Setup(r => r.AddAsync(It.IsAny<CapagFcoParametroEmpresa>()))
                .Callback<CapagFcoParametroEmpresa>(e => saved = e)
                .Returns(Task.CompletedTask);

            var request = new UpsertCapagFcoParametrosRequest
            {
                Bloco = "l100",
                Excecoes = new JsonObject
                {
                    ["1.02.03.01.30"] = new JsonObject { ["trat"] = "T9" }
                },
                VersaoBase = "a1b2c3d4e5f67890"
            };

            var result = await UpsertHandler().Handle(
                new UpsertCapagFcoParametrosCommand { IdEmpresa = 123, Request = request },
                CancellationToken.None);

            saved.Should().NotBeNull();
            saved!.IdEmpresa.Should().Be(123);
            saved.IdTenant.Should().Be(10);
            saved.IdUsuario.Should().Be(99);
            saved.BaseVersaoHash.Should().Be("a1b2c3d4e5f67890");
            saved.ExcecoesL300Json.Should().Be(CapagFcoParametrosHelper.EmptyJsonObject);
            CapagFcoParametrosHelper.ParseExcecoes(saved.ExcecoesL100Json).Count.Should().Be(1);

            result.ContasPersonalizadasL100.Should().Be(1);
            result.ContasPersonalizadasL300.Should().Be(0);
            result.ExcecoesL100["1.02.03.01.30"]!["trat"]!.GetValue<string>().Should().Be("T9");
        }

        [Fact]
        public async Task Put_substitui_apenas_o_bloco_informado()
        {
            SetupEmpresa();
            var existing = new CapagFcoParametroEmpresa
            {
                Id = 1,
                IdEmpresa = 123,
                IdTenant = 10,
                ExcecoesL100Json = """{"1.01":{"trat":"T1"}}""",
                ExcecoesL300Json = """{"3.01":{"acao":"X"}}""",
                BaseVersaoHash = "oldhash",
                CreatedAt = new DateTime(2026, 1, 1),
                UpdatedAt = new DateTime(2026, 1, 1)
            };

            _parametroRepo
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CapagFcoParametroEmpresa, bool>>>()))
                .ReturnsAsync(existing);
            _parametroRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var request = new UpsertCapagFcoParametrosRequest
            {
                Bloco = "l300",
                Excecoes = new JsonObject(),
                VersaoBase = "newhash01234567"
            };

            var result = await UpsertHandler().Handle(
                new UpsertCapagFcoParametrosCommand { IdEmpresa = 123, Request = request },
                CancellationToken.None);

            existing.ExcecoesL100Json.Should().Be("""{"1.01":{"trat":"T1"}}""");
            existing.ExcecoesL300Json.Should().Be("{}");
            existing.BaseVersaoHash.Should().Be("newhash01234567");
            existing.IdUsuario.Should().Be(99);
            result.ContasPersonalizadasL100.Should().Be(1);
            result.ContasPersonalizadasL300.Should().Be(0);
            _parametroRepo.Verify(r => r.Update(existing), Times.Once);
        }
    }
}
