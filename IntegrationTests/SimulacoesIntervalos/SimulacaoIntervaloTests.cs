using Application.Commands.SimulacoesCalc;
using Application.Commands.SimulacoesIntervalo;
using Application.Handlers.SimulacoesCalc;
using Application.Handlers.SimulacoesIntervalo;
using Domain.Entities;
using Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mysqlx.Crud;
using TestSupport.Fakes.SimulacoesCalc;
using TestSupport.Fakes.SimulacoesIntervalos;

namespace IntegrationTests.SimulacoesIntervalos
{
    public class SimulacaoIntervaloTests
    {
        private readonly IMediator _mediator;
        private readonly CPGDbContext _dbContext;

        public SimulacaoIntervaloTests()
        {
            var provider = TestStartup.Provider();
            _dbContext = provider.GetRequiredService<CPGDbContext>();
            _mediator = provider.GetRequiredService<IMediator>();
        }

        [Fact]
        public async Task Create()
        {
            var request = new CreateSimulacaoIntervaloRequestFaker().Generate();
            var command = new CreateSimulacaoIntervaloCommand(request);

            var response = await _mediator.Send(command);
            Assert.Equal(CreateSimulacaoIntervaloHandler.CreateMessage, response.Message);



            //var intervaloSalvo = await _dbContext.Set<SimulacaoIntervalo>().FirstOrDefaultAsync(x =>x.IdSimulacaoCalc == request.IdSimulacaoCalc && x.TipoIntervalo == request.TipoIntervalo);

            //Assert.NotNull(intervaloSalvo);

            //Assert.Equal(request.IdEmpresa, intervaloSalvo.IdEmpresa);
            //Assert.Equal(request.TipoIntervalo, intervaloSalvo.TipoIntervalo);
        }


        [Fact]
        public async Task Delete()
        {
            var command = new DeleteSimulacaoIntervaloCommand(1);
            var response = await _mediator.Send(command);
            Assert.Equal(DeleteSimulacaoIntervaloHandler.DeleteMessage, response.Message);
        }
    }
}