using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries
{
    public class AuthQuery : IRequest<LoginResponse>
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string? DeviceId { get; set; } = string.Empty;
    }
}
