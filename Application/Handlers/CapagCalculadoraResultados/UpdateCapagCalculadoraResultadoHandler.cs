using Application.Commands.CapagCalculadoraResultados;
using Application.Exceptions.CapagCalculadoraResultados;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.CapagCalculadoraResultados
{
    public class UpdateCapagCalculadoraResultadoHandler : IRequestHandler<UpdateCapagCalculadoraResultadoCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<CapagCalculadoraResultado> _baseRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public UpdateCapagCalculadoraResultadoHandler(
            IBaseRepository<CapagCalculadoraResultado> baseRepository,
            ICurrentUserService currentUser,
            IMapper mapper)
        {
            _baseRepository = baseRepository;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<UpdateApiResponse> Handle(UpdateCapagCalculadoraResultadoCommand request, CancellationToken cancellationToken)
        {
            var entity = await _baseRepository.GetByIdAsync(request.Id)
                ?? throw new CapagCalculadoraResultadoNotFoundException(request.Id);

            var modelo = request.Request.Modelo.Trim();
            var exists = await _baseRepository.AnyAsync(e =>
                e.Id != request.Id &&
                e.IdEmpresa == entity.IdEmpresa &&
                e.Modelo == modelo);

            if (exists)
                throw new CapagCalculadoraResultadoConflictException(entity.IdEmpresa, modelo);

            var entityToUpdate = _mapper.Map(request.Request, entity);
            entityToUpdate.Modelo = modelo;
            entityToUpdate.Classificacao = request.Request.Classificacao.Trim().ToUpperInvariant();
            entityToUpdate.IdUsuario = _currentUser.UserId;

            _baseRepository.Update(entityToUpdate);
            await _baseRepository.SaveChangesAsync();

            return new UpdateApiResponse { Message = "Resultado da calculadora CAPAG atualizado com sucesso" };
        }
    }
}
