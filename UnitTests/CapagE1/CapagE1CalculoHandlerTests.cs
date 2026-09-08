using System.Linq.Expressions;
using System.Text.Json.Nodes;
using Application.Commands.CapagE1;
using Application.Exceptions.CapagE1;
using Application.Exceptions.Empresas;
using Application.Handlers.CapagE1;
using Application.Helpers;
using Application.Queries.CapagE1;
using Domain.Contracts.CapagE1;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Interface;
using Moq;

namespace UnitTests.CapagE1
{
    public class CapagE1CalculoHandlerTests
    {
        private readonly Mock<IBaseRepository<Empresa>> _empresaRepo = new();
        private readonly Mock<IBaseRepository<CapagE1Calculo>> _calculoRepo = new();
        private readonly Mock<ICurrentUserService> _currentUser = new();

        public CapagE1CalculoHandlerTests()
        {
            _currentUser.Setup(c => c.TenantId).Returns(10);
            _currentUser.Setup(c => c.UserId).Returns(99);
        }

        private GetCapagE1CalculoHandler GetHandler() =>
            new(_empresaRepo.Object, _calculoRepo.Object, _currentUser.Object);

        private CreateCapagE1CalculoHandler CreateHandler() =>
            new(_empresaRepo.Object, _calculoRepo.Object, _currentUser.Object);

        private UpdateCapagE1CalculoHandler UpdateHandler() =>
            new(_empresaRepo.Object, _calculoRepo.Object, _currentUser.Object);

        private void SetupEmpresa(long idEmpresa = 456, long idTenant = 10)
        {
            _empresaRepo
                .Setup(r => r.GetByIdAsync(idEmpresa))
                .ReturnsAsync(new Empresa { IdEmpresa = idEmpresa, IdTenant = idTenant });
        }

        private static CapagE1CalculoPayload SamplePayload(long idEmpresa = 456) => new()
        {
            IdEmpresa = idEmpresa,
            Modelo = "capag-e-1",
            Anos = JsonNode.Parse("[2023,2024,2025]"),
            Parametros = JsonNode.Parse("""{"horizonteMeses":60,"multiplicadorProjecao":5}"""),
            GreLinhas = JsonNode.Parse("""
                [{
                  "id": "dyn_3_01_01_01",
                  "codigoExtra": "3.01.01.01",
                  "justificativa": "Motivo da inversão...",
                  "valores": { "2023": -32589007.82, "2024": 1, "2025": 1 },
                  "valoresSinalInvertido": { "2023": true },
                  "dinamica": true
                }]
                """),
            PlraLinhas = JsonNode.Parse("[]"),
            ContasManuaisGre = JsonNode.Parse("[]"),
            Resultado = JsonNode.Parse("""{"parcial":true,"total_capag":0}""")
        };

        [Fact]
        public async Task Get_empresa_inexistente_lanca_not_found()
        {
            _empresaRepo
                .Setup(r => r.GetByIdAsync(456))
                .ReturnsAsync((Empresa?)null);

            var act = () => GetHandler().Handle(
                new GetCapagE1CalculoQuery { IdEmpresa = 456 },
                CancellationToken.None);

            await act.Should().ThrowAsync<EmpresaNotFoundException>();
        }

        [Fact]
        public async Task Get_sem_calculo_lanca_not_found()
        {
            SetupEmpresa();
            _calculoRepo
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CapagE1Calculo, bool>>>()))
                .ReturnsAsync((CapagE1Calculo?)null);

            var act = () => GetHandler().Handle(
                new GetCapagE1CalculoQuery { IdEmpresa = 456 },
                CancellationToken.None);

            await act.Should().ThrowAsync<CapagE1CalculoNotFoundException>();
        }

        [Fact]
        public async Task Get_devolve_gre_linhas_com_valoresSinalInvertido()
        {
            SetupEmpresa();
            var payload = SamplePayload();
            var entity = new CapagE1Calculo
            {
                Id = 123,
                IdEmpresa = 456,
                IdTenant = 10,
                Modelo = "capag-e-1",
                PayloadJson = CapagE1CalculoHelper.SerializePayload(payload),
                DateCreate = DateTime.Parse("2026-09-04T12:00:00"),
                DateUpdate = DateTime.Parse("2026-09-04T12:00:00")
            };

            _calculoRepo
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CapagE1Calculo, bool>>>()))
                .ReturnsAsync(entity);

            var result = await GetHandler().Handle(
                new GetCapagE1CalculoQuery { IdEmpresa = 456 },
                CancellationToken.None);

            result.Id.Should().Be(123);
            result.IdEmpresa.Should().Be(456);
            var linha = result.GreLinhas!.AsArray()[0]!.AsObject();
            linha["valoresSinalInvertido"]!["2023"]!.GetValue<bool>().Should().BeTrue();
            linha["valores"]!["2023"]!.GetValue<decimal>().Should().Be(-32589007.82m);
            linha["justificativa"]!.GetValue<string>().Should().Be("Motivo da inversão...");
        }

        [Fact]
        public async Task Post_cria_e_devolve_id_preservando_inversao()
        {
            SetupEmpresa();
            _calculoRepo
                .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<CapagE1Calculo, bool>>>()))
                .ReturnsAsync(false);
            _calculoRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            CapagE1Calculo? saved = null;
            _calculoRepo
                .Setup(r => r.AddAsync(It.IsAny<CapagE1Calculo>()))
                .Callback<CapagE1Calculo>(e =>
                {
                    e.Id = 77;
                    saved = e;
                })
                .Returns(Task.CompletedTask);

            var result = await CreateHandler().Handle(
                new CreateCapagE1CalculoCommand(SamplePayload()),
                CancellationToken.None);

            result.Id.Should().Be(77);
            saved.Should().NotBeNull();
            saved!.PayloadJson.Should().Contain("valoresSinalInvertido");
            saved.PayloadJson.Should().Contain("-32589007.82");

            var roundTrip = CapagE1CalculoHelper.DeserializePayload(saved.PayloadJson);
            roundTrip.GreLinhas!.AsArray()[0]!["valoresSinalInvertido"]!["2023"]!
                .GetValue<bool>().Should().BeTrue();
        }

        [Fact]
        public async Task Post_duplicado_lanca_conflict()
        {
            SetupEmpresa();
            _calculoRepo
                .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<CapagE1Calculo, bool>>>()))
                .ReturnsAsync(true);

            var act = () => CreateHandler().Handle(
                new CreateCapagE1CalculoCommand(SamplePayload()),
                CancellationToken.None);

            await act.Should().ThrowAsync<CapagE1CalculoAlreadyExistsException>();
        }

        [Fact]
        public async Task Put_atualiza_payload_preservando_inversao()
        {
            SetupEmpresa();
            var entity = new CapagE1Calculo
            {
                Id = 77,
                IdEmpresa = 456,
                IdTenant = 10,
                Modelo = "capag-e-1",
                PayloadJson = "{}",
                DateCreate = DateTime.Parse("2026-09-01T10:00:00"),
                DateUpdate = DateTime.Parse("2026-09-01T10:00:00")
            };

            _calculoRepo.Setup(r => r.GetByIdAsync(77)).ReturnsAsync(entity);
            _calculoRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var result = await UpdateHandler().Handle(
                new UpdateCapagE1CalculoCommand(SamplePayload(), 77),
                CancellationToken.None);

            result.Id.Should().Be(77);
            entity.PayloadJson.Should().Contain("valoresSinalInvertido");
            var linha = result.GreLinhas!.AsArray()[0]!.AsObject();
            linha["valoresSinalInvertido"]!["2023"]!.GetValue<bool>().Should().BeTrue();
        }
    }

    public class CapagE1CalculoHelperTests
    {
        [Fact]
        public void RoundTrip_preserva_valoresSinalInvertido_e_sinal_negativo()
        {
            var original = new CapagE1CalculoPayload
            {
                IdEmpresa = 1,
                GreLinhas = JsonNode.Parse("""
                    [{"valores":{"2023":-10},"valoresSinalInvertido":{"2023":true},"justificativa":"x"}]
                    """)
            };

            var json = CapagE1CalculoHelper.SerializePayload(original);
            var restored = CapagE1CalculoHelper.DeserializePayload(json);

            var linha = restored.GreLinhas!.AsArray()[0]!;
            linha["valores"]!["2023"]!.GetValue<decimal>().Should().Be(-10m);
            linha["valoresSinalInvertido"]!["2023"]!.GetValue<bool>().Should().BeTrue();
            linha["justificativa"]!.GetValue<string>().Should().Be("x");
        }
    }
}
