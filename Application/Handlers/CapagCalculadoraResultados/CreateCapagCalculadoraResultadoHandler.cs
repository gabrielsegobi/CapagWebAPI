using Application.Commands.CapagCalculadoraResultados;
using Application.Exceptions.CapagCalculadoraResultados;
using Application.Exceptions.Empresas;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.CapagCalculadoraResultados
{
    public class CreateCapagCalculadoraResultadoHandler : IRequestHandler<CreateCapagCalculadoraResultadoCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<CapagCalculadoraResultado> _baseRepository;
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public CreateCapagCalculadoraResultadoHandler(
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

        public async Task<CreateApiResponse> Handle(CreateCapagCalculadoraResultadoCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<CapagCalculadoraResultado>(request.Request)
                ?? throw new InvalidDataException("Invalid data");

            var empresa = await _empresaRepository.GetByIdAsync(request.Request.IdEmpresa)
                ?? throw new EmpresaNotFoundException(request.Request.IdEmpresa);

            var modelo = request.Request.Modelo.Trim();
            var exists = await _baseRepository.AnyAsync(e =>
                e.IdEmpresa == request.Request.IdEmpresa && e.Modelo == modelo);

            if (exists)
                throw new CapagCalculadoraResultadoConflictException(request.Request.IdEmpresa, modelo);

            entity.Modelo = modelo;
            entity.Classificacao = request.Request.Classificacao.Trim().ToUpperInvariant();
            entity.IdUsuario = _currentUser.UserId;

            await _baseRepository.AddAsync(entity);

            // Ao concluir o cálculo, seta status comercial se ainda não definido
            if (!request.Request.Parcial && empresa.Status == null)
            {
                empresa.Status = StatusComercialEmpresa.CalculoEfetuado;
                empresa.UpdatedAt = DateTimeHelper.GetDateTimeNow();
                _empresaRepository.Update(empresa);
            }

            await _baseRepository.SaveChangesAsync();

            return new CreateApiResponse("Resultado da calculadora CAPAG criado com sucesso", entity.Id);
        }
    }
}
