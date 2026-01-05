using Application.Commands.RegDarf;
using Application.Exceptions.RegDarf;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegDarf
{
    public class UpdateRegDarfHandler : IRequestHandler<UpdateRegDarfCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<RegDarfs> _baseRepository;
        private readonly IMapper _mapper;

        public UpdateRegDarfHandler(IBaseRepository<RegDarfs> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<UpdateApiResponse> Handle(UpdateRegDarfCommand request, CancellationToken cancellationToken)
        {
            var darf = await _baseRepository.GetByIdAsync(request.Id) ?? throw new RegDarfNotFoundException(request.Id);

            var darfToUpdate = _mapper.Map(request.Request, darf);

            _baseRepository.Update(darfToUpdate);
            await _baseRepository.SaveChangesAsync();

            return new UpdateApiResponse { Message = "Darf atualizado com sucesso." };
        }
    }
}
