using Application.Commands.ValorCalcVariaveis;
using Application.Filters;
using Application.Handlers.ValorCalcVariaveis;
using Application.Queries.ValorCalcVariaveis;
using Domain.Contracts.ValorCalcVariaveis;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace IntegrationTests.ValorCalcVariavelTests
{
    public class ValorCalcVariavelTest
    {
        private readonly IMediator _mediator;
        private readonly CreateValorCalcVariavelHandler _handler;
        private readonly CPGDbContext _dbContext;

        public ValorCalcVariavelTest()
        {
            var provider = TestStartup.Provider();
            _handler = provider.GetRequiredService<CreateValorCalcVariavelHandler>();
            _dbContext = provider.GetRequiredService<CPGDbContext>();
            _mediator = provider.GetRequiredService<IMediator>();
        }

        [Fact]
        public async Task Adicionar()
        {
            var calc = new CreateValorCalcVariavelRequest
            {
                AnoBase = 2020,
                IdEmpresa = 1,
                IdTipoGrupo = 1,
                IdVariavel = "123",
                Status = "ignorado",
                Valor = 102
            };

            var command = new CreateValorCalcVariavelCommand(calc);
            //var handler.H(command, CancellationToken.None);
            //var response = await _handler.Handle(command, CancellationToken.None);
            var response = await _mediator.Send(command);
            //var response = await _mediator.Send(command);
            Assert.Equal("Valor Criado com sucesso", response.Message);
        }

        [Fact]
        public async Task GetAll()
        {
            var filter = new ValorCalcVariavelFilter
            {
            };

            var command = new GetAllValorCalcVariavelQuery(filter);
            var response = await _mediator.Send(command);

            Assert.Equal(1, response.Paging.Page);
            Assert.True(response.Paging.PageSize <= 10, $"PageSize esperado <= 10, mas foi {response.Paging.PageSize}");
            var items = response.Data.ToList();
            Assert.True(
                items.SequenceEqual(items.OrderBy(x => x.IdValorCalcVariavel)),
                "Os registros não estão ordenados por Id em ordem crescente"
            );


        }
        [Theory]
        [InlineData(1, -1, -1)]
        [InlineData(-1, 2, -1)]
        [InlineData(1, 2, -1)]
        public async Task Handler_deve_filtrar_dados_corretamente(
            long idEmpresa,
            long idTipoGrupo,
            decimal valor)
        {
            var filter = new ValorCalcVariavelFilter
            {
                IdEmpresa = idEmpresa,
                IdTipoGrupo = idTipoGrupo,
                Valor = valor,
                Page = 1,
                PageSize = 10
            };

            var query = new GetAllValorCalcVariavelQuery(filter);

            //var repo = new FakeBaseRepository<ValorCalcVariavel>(FakeDatabase());
            //var mapper = CreateMapper();
            //var handler = new GetAllValorCalcVariavelHandler(repo, mapper);

            //var response = await handler.Handle(query, CancellationToken.None);
            var response = await _mediator.Send(query);
            if (idEmpresa >= 0)
                response.Data.Should().OnlyContain(x => x.IdEmpresa == idEmpresa);

            if (idTipoGrupo >= 0)
                response.Data.Should().OnlyContain(x => x.IdTipoGrupo == idTipoGrupo);

            if (valor >= 0)
                response.Data.Should().OnlyContain(x => x.Valor >= valor);
        } 

        [Theory]
        [InlineData("IdEmpresa", false)]
        [InlineData("IdEmpresa", true)]
        [InlineData("Valor", false)]
        [InlineData("Valor", true)]
        [InlineData("AnoBase", false)]
        [InlineData("AnoBase", true)]
        [InlineData("IdTipoGrupo", false)]
        [InlineData("IdTipoGrupo", true)]
        [InlineData("IdVariavel", false)]
        [InlineData("IdVariavel", true)]
        [InlineData("Status", false)]
        [InlineData("Status", true)]
        public async Task Handler_deve_ordenar_dados_corretamente(string sortBy, bool orderByDescending)
        {
            var filter = new ValorCalcVariavelFilter
            {
                Sort = sortBy,
                OrderByDescending = orderByDescending,
                Page = 1,
                PageSize = 10
            };

            var query = new GetAllValorCalcVariavelQuery(filter);

            var response = await _mediator.Send(query);

            response.Data.Should().NotBeEmpty();

            // Função para obter a propriedade dinamicamente
            object GetPropertyValue(ValorCalcVariavelDto x) =>
                typeof(ValorCalcVariavelDto).GetProperty(sortBy)?.GetValue(x)
                ?? throw new InvalidOperationException($"Propriedade {sortBy} não encontrada");

            var values = response.Data.Select(GetPropertyValue).ToList();

            if (orderByDescending)
                values.Should().BeInDescendingOrder();
            else
                values.Should().BeInAscendingOrder();
        }

    }
}
