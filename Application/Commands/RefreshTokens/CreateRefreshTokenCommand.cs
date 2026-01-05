using Domain.Contracts.RefreshTokens;
using Domain.Entities;
using MediatR;

namespace Application.Commands.RefreshTokens
{
    public class CreateRefreshTokenCommand: IRequest<RefreshToken>
    {
        public CreateRefreshTokenRequest CreateRefreshTokenRequest { get; set; } = new CreateRefreshTokenRequest();
    }
}
