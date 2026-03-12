using Application.Commands.RegsPgdasd;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegsPgdasd
{
    public class CreateRegPgdasdHandler : IRequestHandler<CreateRegPgdasdCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<RegFileName> _fileNameRepository;
        private readonly IMapper _mapper;

        public CreateRegPgdasdHandler(IBaseRepository<RegFileName> fileNameRepository, IMapper mapper)
        {
            _fileNameRepository = fileNameRepository;
            _mapper = mapper;
        }

        public async Task<CreateApiResponse> Handle(CreateRegPgdasdCommand request, CancellationToken cancellationToken)
        {

            var pgdasdList = _mapper.Map<RegFileName>(request.Request) ?? throw new InvalidDataException("Invalid data");

            await _fileNameRepository.AddAsync(pgdasdList);
            await _fileNameRepository.SaveChangesAsync();

            return new CreateApiResponse("Pgdasds Criados com Sucesso" , pgdasdList.Id);
        }
    }
}
