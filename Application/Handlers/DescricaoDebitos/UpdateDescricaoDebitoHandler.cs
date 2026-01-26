using Application.Commands.DescricaoDebitos;
using Application.Exceptions.DescricaoDebitos;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.DescricaoDebitos
{
    public class UpdateDescricaoDebitoHandler : IRequestHandler<UpdateDescricaoDebitoCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<DescricaoDebito> _baseRepository;
        private readonly IMapper _mapper;

        public UpdateDescricaoDebitoHandler(IBaseRepository<DescricaoDebito> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<UpdateApiResponse> Handle(UpdateDescricaoDebitoCommand request, CancellationToken cancellationToken)
        {
            var debito = await _baseRepository.GetByIdAsync(request.Id)
                ?? throw new DescricaoDebitoNotFoundException(request.Id);

            var debitoToUpdate = _mapper.Map(request.Request, debito);

            _baseRepository.Update(debitoToUpdate);
            await _baseRepository.SaveChangesAsync();

            return new UpdateApiResponse { Message = "Descrição do Débito atualizado com sucesso" };
        }
    }
}
