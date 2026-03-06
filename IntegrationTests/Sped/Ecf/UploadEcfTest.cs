using Application.Commands.SPED.ECF;
using Application.Handlers.SPED.ECF;
using Application.Handlers.ValorCalcVariaveis;
using Infrastructure.Context;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TestSupport.Fakes;
namespace IntegrationTests.Sped.Ecf
{
    public class UploadEcfTest
    {
        private readonly IMediator _mediator;
        private readonly UploadECFHandler _handler;
        private readonly CPGDbContext _dbContext;

        public UploadEcfTest()
        {
            var provider = TestStartup.Provider();
            _handler = provider.GetRequiredService<UploadECFHandler>();
            _dbContext = provider.GetRequiredService<CPGDbContext>();
            _mediator = provider.GetRequiredService<IMediator>();
        }


        //[Fact]
        //public async Task Handle_Deve_Processar_Arquivo_ECF_Real()
        //{
        //    // ARRANGE
        //    var caminhoArquivo = @"C:\temp\arquivo_teste.ecf";
        //    var file = FakeFormFile.Create(caminhoArquivo);

        //    var request = new UploadECFRequest
        //    {
        //        File = file
        //    };


        //    var command = new UploadECFCommand(request);

        //    await _mediator.Send(command);

        //    // ACT
        //    //await handler.Handle(command, CancellationToken.None);

        //    // ASSERT
        //    // validações aqui
        //}
    }
}
