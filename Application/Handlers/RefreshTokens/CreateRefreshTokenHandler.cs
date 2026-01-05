using Application.Commands.RefreshTokens;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RefreshTokens
{
    public class CreateRefreshTokenHandler : IRequestHandler<CreateRefreshTokenCommand, RefreshToken>
    {
        private readonly IBaseRepository<RefreshToken> _baseRepository;
        private readonly IMapper _mapper;

        public CreateRefreshTokenHandler(IBaseRepository<RefreshToken> baseRepository, IMapper mapper, IPasswordHasher passwordHasher)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<RefreshToken> Handle(CreateRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshtoken = _mapper.Map<RefreshToken>(request.CreateRefreshTokenRequest);

            if (refreshtoken == null)
            {
                throw new InvalidDataException("Invalid data");
            }


            await _baseRepository.AddAsync(refreshtoken);
            await _baseRepository.SaveChangesAsync();

            return refreshtoken;
        }
    }
}
