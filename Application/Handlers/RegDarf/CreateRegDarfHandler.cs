using Application.Commands.RegDarf;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegDarf
{
    public class CreateRegDarfHandler : IRequestHandler<CreateRegDarfCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<RegFileName> _fileNameRepository;
        private readonly IMapper _mapper;

        public CreateRegDarfHandler(IBaseRepository<RegFileName> filename, IMapper mapper)
        {
            _fileNameRepository = filename;
            _mapper = mapper;
        }
        public async Task<CreateApiResponse> Handle(CreateRegDarfCommand request, CancellationToken cancellationToken)
        {
            var darfList = _mapper.Map<RegFileName>(request.Requests) ?? throw new InvalidDataException("Invalid data");

            await _fileNameRepository.AddAsync(darfList);
            await _fileNameRepository.SaveChangesAsync();

            return new CreateApiResponse
            {
                Message = "Darfs Criadas com Sucesso"
            };
        }
    }
}
