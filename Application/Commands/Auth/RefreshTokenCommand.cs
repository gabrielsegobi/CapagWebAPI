using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.Auth
{
    public class RefreshTokenCommand: IRequest<LoginResponse>
    {
        public string Token { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
    }
}
