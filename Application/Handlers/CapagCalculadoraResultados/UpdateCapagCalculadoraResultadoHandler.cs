using Application.Commands.CapagCalculadoraResultados;
using Application.Exceptions.CapagCalculadoraResultados;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.CapagCalculadoraResultados
{
    public class UpdateCapagCalculadoraResultadoHandler : IRequestHandler<UpdateCapagCalculadoraResultadoCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<CapagCalculadoraResultado> _baseRepository;
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public UpdateCapagCalculadoraResultadoHandler(
            IBaseRepository<CapagCalculadoraResultado> baseRepository,
            IBaseRepository<Empresa> empresaRepository,
            ICurrentUserService currentUser,
            IMapper mapper)
        {
            _baseRepository = baseRepository;
            _empresaRepository = empresaRepository;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<UpdateApiResponse> Handle(UpdateCapagCalculadoraResultadoCommand request, CancellationToken cancellationToken)
        {
            var entity = await _baseRepository.GetByIdAsync(request.Id)
                ?? throw new CapagCalculadoraResultadoNotFoundException(request.Id);

            var modelo = request.Request.Modelo.Trim();
            var entityToUpdate = _mapper.Map(request.Request, entity);
            entityToUpdate.Modelo = modelo;
            entityToUpdate.Classificacao = request.Request.Classificacao.Trim().ToUpperInvariant();
            entityToUpdate.IdUsuario = _currentUser.UserId;

            _baseRepository.Update(entityToUpdate);

            // Ao concluir o cálculo, seta status comercial se ainda não definido
            if (!request.Request.Parcial)
            {
                var empresa = await _empresaRepository.GetByIdAsync(entity.IdEmpresa);
                if (empresa != null && empresa.Status == null)
                {
                    empresa.Status = StatusComercialEmpresa.CalculoEfetuado;
                    empresa.UpdatedAt = DateTimeHelper.GetDateTimeNow();
                    _empresaRepository.Update(empresa);
                }
            }

            await _baseRepository.SaveChangesAsync();

            return new UpdateApiResponse { Message = "Resultado da calculadora CAPAG atualizado com sucesso" };
        }
    }
}
