using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.RefreshTokens
{
    public class RefreshTokenCommand : IRequest<LoginResponse>
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
    }
}
