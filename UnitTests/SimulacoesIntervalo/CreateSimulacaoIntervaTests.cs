using Application.Commands.SimulacoesIntervalo;
using Application.Exceptions.Empresas;
using Application.Exceptions.SimulacoesIntervalo;
using Application.Handlers.SimulacoesIntervalo;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Interface;
using Moq;
using System.Linq.Expressions;
using TestSupport.Fakes.SimulacoesIntervalos;

namespace UnitTests.SimulacoesIntervalo
{
    public class CreateSimulacaoIntervaTests
    {
        private readonly Mock<IBaseRepository<SimulacaoIntervalo>> _intervaloRepositoryMock;
        private readonly Mock<IBaseRepository<Empresa>> _empresaRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly CreateSimulacaoIntervaloHandler _handler;

        public CreateSimulacaoIntervaTests()
        {
            _intervaloRepositoryMock = new Mock<IBaseRepository<SimulacaoIntervalo>>();
            _empresaRepositoryMock = new Mock<IBaseRepository<Empresa>>();
            _mapperMock = new Mock<IMapper>();

            _handler = new CreateSimulacaoIntervaloHandler(
                _intervaloRepositoryMock.Object,
                _mapperMock.Object,
                _empresaRepositoryMock.Object
            );
        }
        [Fact]
        public async Task Handle_Deve_Criar_Intervalo_Com_Sucesso()
        {
            // ARRANGE
            var request = new CreateSimulacaoIntervaloRequestFaker().Generate();
            var command = new CreateSimulacaoIntervaloCommand(request);
            var intervalo = new SimulacaoIntervalo
            {
                IdEmpresa = request.IdEmpresa,
                IdSimulacaoCalc = request.IdSimulacaoCalc,
                TipoIntervalo = request.TipoIntervalo
            };


            _mapperMock.Setup(m => m.Map<SimulacaoIntervalo>(request)).Returns(intervalo);
            _empresaRepositoryMock.Setup(r => r.GetByIdAsync(request.IdEmpresa)).ReturnsAsync(new Empresa { IdEmpresa = request.IdEmpresa });
            _intervaloRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<SimulacaoIntervalo, bool>>>())).ReturnsAsync((SimulacaoIntervalo?)null);

            // ACT

            var response = await _handler.Handle(command, CancellationToken.None);

            // ASSERT

            Assert.Equal(CreateSimulacaoIntervaloHandler.CreateMessage, response.Message);

            _intervaloRepositoryMock.Verify(r => r.AddAsync(intervalo), Times.Once);
            _empresaRepositoryMock.Verify(r => r.GetByIdAsync(request.IdEmpresa), Times.Once);
            _mapperMock.Verify(m => m.Map<SimulacaoIntervalo>(request), Times.Once);
            _intervaloRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }


        [Fact]
        public async Task Handle_Deve_Lancar_Exception_Quando_Empresa_Nao_Existir()
        {
            // ARRANGE
            var request = new CreateSimulacaoIntervaloRequestFaker().Generate();
            var command = new CreateSimulacaoIntervaloCommand(request);

            _mapperMock.Setup(m => m.Map<SimulacaoIntervalo>(request)).Returns(new SimulacaoIntervalo());
            _empresaRepositoryMock.Setup(r => r.GetByIdAsync(request.IdEmpresa)).ReturnsAsync((Empresa?)null);

            // ACT + ASSERT
            await Assert.ThrowsAsync<EmpresaNotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );

            _intervaloRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SimulacaoIntervalo>()), Times.Never);
            _intervaloRepositoryMock.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<SimulacaoIntervalo, bool>>>()), Times.Never);
            _empresaRepositoryMock.Verify(r => r.GetByIdAsync(request.IdEmpresa), Times.Once);
            _mapperMock.Verify(m => m.Map<SimulacaoIntervalo>(request), Times.Once);
            _intervaloRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }


        [Fact]
        public async Task Handle_Deve_Lancar_Exception_Quando_Ja_Existir_Entrada()
        {
            // ARRANGE

            var request = CreateSimulacaoIntervaloRequestFaker.Entrada();

            var command = new CreateSimulacaoIntervaloCommand(request);

            _mapperMock.Setup(m => m.Map<SimulacaoIntervalo>(request)).Returns(new SimulacaoIntervalo());

            _empresaRepositoryMock.Setup(r => r.GetByIdAsync(request.IdEmpresa)).ReturnsAsync(new Empresa { IdEmpresa = request.IdEmpresa });

            _intervaloRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<SimulacaoIntervalo, bool>>>())).ReturnsAsync(new SimulacaoIntervalo());

            // ACT + ASSERT
            await Assert.ThrowsAsync<DuplicateIntervalException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );

            _intervaloRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SimulacaoIntervalo>()), Times.Never);
            _intervaloRepositoryMock.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<SimulacaoIntervalo, bool>>>()), Times.Once);
            _empresaRepositoryMock.Verify(r => r.GetByIdAsync(request.IdEmpresa), Times.Once);
            _mapperMock.Verify(m => m.Map<SimulacaoIntervalo>(request), Times.Once);
            _intervaloRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}

