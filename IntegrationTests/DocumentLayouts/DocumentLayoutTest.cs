using Application.Commands.DocumentLayouts;
using Application.Commands.Empresas;
using Application.Commands.SimulacoesCalc;
using Application.Handlers.DocumentLayouts;
using Application.Handlers.SimulacoesCalc;
using Domain.Entities;
using Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestSupport.Fakes.DocumentLayouts;
using TestSupport.Fakes.SimulacoesCalc;

namespace IntegrationTests.DocumentLayouts
{
    public class DocumentLayoutTest
    {
        private readonly IMediator _mediator;
        private readonly CPGDbContext _dbContext;
        protected readonly DbSet<DocumentLayout> _dbSet;

        public DocumentLayoutTest()
        {
            var provider = TestStartup.Provider();
            _dbContext = provider.GetRequiredService<CPGDbContext>();
            _mediator = provider.GetRequiredService<IMediator>();
            _dbSet = _dbContext.Set<DocumentLayout>();
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Create_Should_Create_With_Or_Without_ValidationRegex(bool hasValidationRegex)
        {
            var faker = new CreateDocumentLayoutRequestFaker();

            var request = hasValidationRegex
                ? faker.WithValidationRegex().Generate()
                : faker.WithoutValidationRegex().Generate();

            var command = new CreateDocumentLayoutCommand(request);

            var response = await _mediator.Send(command);

            Assert.Equal(CreateDocumentLayoutHandler.CreateMessage, response.Message);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Update_Should_Update_With_Or_Without_ValidationRegex(bool hasValidationRegex)
        {
            // ============================
            // ARRANGE
            // ============================

            var createFaker = new CreateDocumentLayoutRequestFaker();
            

            var createRequest = createFaker
                .WithValidationRegex()
                .Generate();

            var createCommand = new CreateDocumentLayoutCommand(createRequest);

            var createResponse = await _mediator.Send(createCommand);

            // Recupera o layout criado
            var layout = await _dbSet
                .AsNoTracking()
                .Include(x => x.ValidationRegexes)
                .SingleAsync(x => x.LayoutName == createRequest.LayoutName);

            var originalRegexIds = layout.ValidationRegexes?
                    .Select(x => x.Id)
                    .ToList();


            var updateFaker = new UpdateDocumentLayoutRequestFaker();

            var updateRequest = hasValidationRegex
                ? updateFaker.WithValidationRegex().Generate()
                : updateFaker.WithoutValidationRegex().Generate();


            var updateCommand = new UpdateDocumentLayoutCommand(layout.Id, updateRequest);

            // ============================
            // ACT
            // ============================

            var response = await _mediator.Send(updateCommand);

            // ============================
            // ASSERT
            // ============================

            Assert.Equal(UpdateDocumentLayoutHandler.UpdateMessage, response.Message);

            var updatedLayout = await _dbSet
              .Include(x => x.ValidationRegexes)
              .FirstAsync(x => x.Id == layout.Id);

            Assert.Equal(updateRequest.LayoutName, updatedLayout.LayoutName);
            Assert.Equal(updateRequest.ValidationRegex, updatedLayout.ValidationRegex);
            Assert.Equal(updateRequest.System, updatedLayout.System);

            if (hasValidationRegex)
            {
                Assert.NotNull(updatedLayout.ValidationRegexes);
                Assert.NotEmpty(updatedLayout.ValidationRegexes);

                Assert.Equal(
                            updateRequest.ValidationRegexes!.Count,
                            updatedLayout.ValidationRegexes.Count
                        );

                if (originalRegexIds != null && originalRegexIds.Any())
                {
                    Assert.False(
                        updatedLayout.ValidationRegexes
                            .Any(x => originalRegexIds.Contains(x.Id)),
                        "As regex antigas não deveriam permanecer após o update."
                    );
                }

                var updatedRegexValues = updatedLayout.ValidationRegexes
                           .Select(x => x.Regex)
                           .ToList();

                foreach (var regex in updateRequest.ValidationRegexes!)
                {
                    Assert.Contains(regex.Regex, updatedRegexValues);
                }
            }
            else
            {
                Assert.True(
                    updatedLayout.ValidationRegexes == null
                    || !updatedLayout.ValidationRegexes.Any(),
                    "ValidationRegexes deveria estar vazia ou null quando não enviada."
                );
            }
        }
    }
}
