using Application.Commands.CodigosRegistroDescricao;
using Application.Exceptions.CodigosRegistroDescricao;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.CodigosRegistroDescricao
{
    public class UpdateCodigoRegistroDescricaoHandler : IRequestHandler<UpdateCodigoRegistroDescricaoCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<CodigoRegistroDescricao> _baseRepository;
        private readonly IMapper _mapper;

        public UpdateCodigoRegistroDescricaoHandler(IBaseRepository<CodigoRegistroDescricao> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<UpdateApiResponse> Handle(UpdateCodigoRegistroDescricaoCommand request, CancellationToken cancellationToken)
        {
            var entity = await _baseRepository.GetByIdAsync(request.Id)
                ?? throw new CodigoRegistroDescricaoNotFoundException(request.Id);

            var entityToUpdate = _mapper.Map(request.Request, entity);

            _baseRepository.Update(entityToUpdate);
            await _baseRepository.SaveChangesAsync();

            return new UpdateApiResponse { Message = "Código de registro descrição atualizado com sucesso" };
        }
    }
}
