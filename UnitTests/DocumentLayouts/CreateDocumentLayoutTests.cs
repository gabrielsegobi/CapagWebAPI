using Application.Commands.DocumentLayouts;
using Application.Exceptions.DocumentLayouts;
using Application.Handlers.DocumentLayouts;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Interface;
using Moq;
using System.Linq.Expressions;
using TestSupport.Fakes.DocumentLayouts;

namespace UnitTests.DocumentLayouts
{
    public class CreateDocumentLayoutTests
    {
        private readonly Mock<IBaseRepository<DocumentLayout>> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly CreateDocumentLayoutHandler _handler;

        public CreateDocumentLayoutTests()
        {
            _repositoryMock = new Mock<IBaseRepository<DocumentLayout>>();
            _mapperMock = new Mock<IMapper>();

            _handler = new CreateDocumentLayoutHandler(
                _repositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Handle_Deve_Criar_Layout_Com_Sucesso()
        {
            // ARRANGE
            var request = new CreateDocumentLayoutRequestFaker().Generate();
            var command = new CreateDocumentLayoutCommand(request);

            var layout = new DocumentLayout
            {
                LayoutName = request.LayoutName
            };

            _mapperMock
                .Setup(m => m.Map<DocumentLayout>(request))
                .Returns(layout);

            _repositoryMock
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<DocumentLayout, bool>>>()))
                .ReturnsAsync((DocumentLayout?)null);

            // ACT
            var response = await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.Equal(CreateDocumentLayoutHandler.CreateMessage, response.Message);

            _mapperMock.Verify(m => m.Map<DocumentLayout>(request), Times.Once);
            _repositoryMock.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<DocumentLayout, bool>>>()), Times.Once);
            _repositoryMock.Verify(r => r.AddAsync(layout), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }


        [Fact]
        public async Task Handle_Deve_Lancar_Exception_Quando_Layout_Ja_Existir()
        {
            // ARRANGE
            var request = new CreateDocumentLayoutRequestFaker().Generate();
            var command = new CreateDocumentLayoutCommand(request);

            var layout = new DocumentLayout
            {
                LayoutName = request.LayoutName
            };

            _mapperMock
                .Setup(m => m.Map<DocumentLayout>(request))
                .Returns(layout);

            _repositoryMock
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<DocumentLayout, bool>>>()))
                .ReturnsAsync(new DocumentLayout());

            // ACT + ASSERT
            await Assert.ThrowsAsync<LayoutNameConflictException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );

            _mapperMock.Verify(m => m.Map<DocumentLayout>(request), Times.Once);
            _repositoryMock.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<DocumentLayout, bool>>>()), Times.Once);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<DocumentLayout>()), Times.Never);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }


        [Fact]
        public async Task Handle_Deve_Lancar_Exception_Quando_Map_Retornar_Null()
        {
            // ARRANGE
            var request = new CreateDocumentLayoutRequestFaker().Generate();
            var command = new CreateDocumentLayoutCommand(request);

            _mapperMock
                .Setup(m => m.Map<DocumentLayout>(request))
                .Returns((DocumentLayout?)null);

            // ACT + ASSERT
            await Assert.ThrowsAsync<InvalidDataException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );

            _mapperMock.Verify(m => m.Map<DocumentLayout>(request), Times.Once);
            _repositoryMock.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<DocumentLayout, bool>>>()), Times.Never);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<DocumentLayout>()), Times.Never);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
