using Application.Commands.DescricaoDebitos;
using Application.Handlers.DescricaoDebitos;
using Infrastructure.Context;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.DescricaoDebitos
{
    public class DescricaoDebitoTest
    {
        private readonly IMediator _mediator;
        private readonly CPGDbContext _dbContext;

        public DescricaoDebitoTest()
        {
            var provider = TestStartup.Provider();
            _dbContext = provider.GetRequiredService<CPGDbContext>();
            _mediator = provider.GetRequiredService<IMediator>();
        }

        [Fact]
        public async Task Delete()
        {
            var command = new DeleteDescricaoDebitoCommand(2);
            var response = await _mediator.Send(command);
            Assert.Equal(DeleteDescricaoDebitoHandler.DeleteMessage, response.Message);
        }
    }
}
