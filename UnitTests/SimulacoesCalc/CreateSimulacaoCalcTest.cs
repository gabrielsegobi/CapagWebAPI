using Application.Commands.SimulacoesCalc;
using Application.Handlers.SimulacoesCalc;
using AutoMapper;
using Domain.Contracts.SimulacoesCalc;
using Domain.Entities;
using Infrastructure.Interface;
using Moq;
using System.Reflection.Metadata;

namespace UnitTests.SimulacoesCalc
{
    public class CreateSimulacaoCalcTest
    {
        private readonly Mock<IBaseRepository<SimulacaoCalc>> _simulacaoRepositoryMock;
        private readonly Mock<IBaseRepository<Empresa>> _empresaRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly CreateSimulacaoCalcHandler _handler;


        public CreateSimulacaoCalcTest()
        {
            _simulacaoRepositoryMock = new Mock<IBaseRepository<SimulacaoCalc>>();
            _empresaRepositoryMock = new Mock<IBaseRepository<Empresa>>();
            _mapperMock = new Mock<IMapper>();

            _handler = new CreateSimulacaoCalcHandler(
              _simulacaoRepositoryMock.Object,
              _empresaRepositoryMock.Object,
              _mapperMock.Object);
        }


        [Fact]
        public async Task Handle_Deve_Criar_Simulacao_Com_Sucesso()
        {
            // ARRANGE
            var request = new CreateSimulacaoCalcRequest
            {
                IdEmpresa = 1,
                TipoSimulacao = "TESTE"
            };

            var command = new CreateSimulacaoCalcCommand(request);

            var simulacao = new SimulacaoCalc
            {
                IdEmpresa = request.IdEmpresa,
                TipoSimulacao = request.TipoSimulacao
            };

            _mapperMock
                .Setup(m => m.Map<SimulacaoCalc>(request))
                .Returns(simulacao);

            _empresaRepositoryMock
                .Setup(r => r.GetByIdAsync(request.IdEmpresa))
                .ReturnsAsync(new Empresa { IdEmpresa = request.IdEmpresa });

            // ACT
            var response = await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.Equal(CreateSimulacaoCalcHandler.CreateMessage, response.Message);

            _simulacaoRepositoryMock.Verify(r => r.AddAsync(simulacao), Times.Once);
            _simulacaoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }


    }
}
