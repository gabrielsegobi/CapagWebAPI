using Application.Commands.SimulacoesCalc;
using Application.Filters;
using Application.Handlers.SimulacoesCalc;
using Application.Handlers.ValorCalcVariaveis;
using Application.Queries.SimulacoesCalc;
using Infrastructure.Context;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;
using TestSupport.Fakes.SimulacoesCalc;

namespace IntegrationTests.SimulacoesCalc
{
    public class SimulacaoCalcTest
    {
        private readonly IMediator _mediator;
        private readonly CPGDbContext _dbContext;

        public SimulacaoCalcTest()
        {
            var provider = TestStartup.Provider();
            _dbContext = provider.GetRequiredService<CPGDbContext>();
            _mediator = provider.GetRequiredService<IMediator>();
        }

        [Fact]
        public async Task Create()
        {
            var request = new CreateSimulacaoCalcRequestFaker().Generate();
            var command = new CreateSimulacaoCalcCommand(request);

            var response = await _mediator.Send(command);
            Assert.Equal(CreateSimulacaoCalcHandler.CreateMessage, response.Message);
        }

        [Fact]
        public async Task Update()
        {
            var request = new UpdateSimulacaoCalcRequestFaker().Generate();
            var command = new UpdateSimulacaoCalcCommand(1, request);

            var response = await _mediator.Send(command);
            Assert.Equal(UpdateSimulacaoCalcHandler.UpdateMessage, response.Message);
        }

        [Fact]
        public async Task GetAll()
        {
            var request = new SimulacaoCalcFilter();
            var command = new GetAllSimulacoesCalcQuery(request);

            var response = await _mediator.Send(command);
            Assert.Equal(1, response.Paging.Page);
            Assert.True(response.Paging.PageSize <= 10);

        }
    }
}
