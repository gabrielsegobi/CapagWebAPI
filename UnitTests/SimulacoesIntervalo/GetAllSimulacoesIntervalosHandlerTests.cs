using Application.Filters;
using Application.Handlers.SimulacoesIntervalo;
using Application.Queries.SimulacoesCalc;
using AutoBogus;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Contracts.SimulacoesIntervalo;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Interface;
using Moq;
using System.Linq.Expressions;

namespace UnitTests.SimulacoesIntervalo
{
    public class GetAllSimulacoesIntervalosHandlerTests
    {
        private readonly Mock<IBaseRepository<SimulacaoIntervalo>> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly GetAllSimulacoesIntervalosHandler _handler;

        public GetAllSimulacoesIntervalosHandlerTests()
        {
            _repositoryMock = new Mock<IBaseRepository<SimulacaoIntervalo>>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetAllSimulacoesIntervalosHandler(
                _repositoryMock.Object,
                _mapperMock.Object
            );

        }
        [Theory]
        [InlineData(1, -1, -1, -1, -1)]
        [InlineData(-1, 10, -1, -1, -1)]
        [InlineData(1, 10, -1, -1, -1)]
        [InlineData(1, 10, 1, -1, -1)]
        [InlineData(1, 10, 1, 12, -1)]
        [InlineData(1, 10, 1, 12, 2.5)]
        public async Task Handler_deve_filtrar_dados_corretamente(
     long idEmpresa,
     long idSimulacaoCalc,
     int mesIni,
     int mesFim,
     decimal pctMensal)
        {
            // Arrange
            var filter = new SimulacaoIntervaloFilter
            {
                IdEmpresa = idEmpresa,
                IdSimulacaoCalc = idSimulacaoCalc,
                MesIni = mesIni,
                MesFim = mesFim,
                PctMensal = pctMensal,
                Page = 1,
                PageSize = 20
            };

            var query = new GetAllSimulacoesIntervalosQuery(filter);

            var entidades = AutoFaker.Generate<SimulacaoIntervalo>(30).AsQueryable();
            var dtos = AutoFaker.Generate<SimulacaoIntervaloDto>(30);

            _repositoryMock
                     .Setup(r => r.Query(
                         It.IsAny<Expression<Func<SimulacaoIntervalo, bool>>>(),
                         It.IsAny<bool>()))
                     .Returns(entidades);



            _mapperMock
                .Setup(m => m.Map<IEnumerable<SimulacaoIntervaloDto>>(
                    It.IsAny<IEnumerable<SimulacaoIntervalo>>()))
                .Returns(dtos);



            // Act
            var response = await _handler.Handle(query, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Data.Should().NotBeNull();

            if (idEmpresa >= 0)
                response.Data.Should().OnlyContain(x => x.IdEmpresa == idEmpresa);

            if (idSimulacaoCalc >= 0)
                response.Data.Should().OnlyContain(x => x.IdSimulacaoCalc == idSimulacaoCalc);

            if (mesIni >= 0)
                response.Data.Should().OnlyContain(x => x.MesIni == mesIni);

            if (mesFim >= 0)
                response.Data.Should().OnlyContain(x => x.MesFim == mesFim);

            if (pctMensal >= 0)
                response.Data.Should().OnlyContain(x => x.PctMensal >= pctMensal);
        }
    }
}
