using Application.Commands.ValorCalcVariaveis;
using Application.Exceptions.Empresas;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.ValorCalcVariaveis
{
    public class CreateValorCalcVariavelHandler : IRequestHandler<CreateValorCalcVariavelCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<ValorCalcVariavel> _baseRepository;
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IMapper _mapper;

        public CreateValorCalcVariavelHandler(IBaseRepository<ValorCalcVariavel> baseRepository, IBaseRepository<Empresa> empresaRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _empresaRepository = empresaRepository;
            _mapper = mapper;
        }

        public async Task<CreateApiResponse> Handle(CreateValorCalcVariavelCommand request, CancellationToken cancellationToken)
        {
            _ = await _empresaRepository.GetByIdAsync(request.Request.IdEmpresa)
                ?? throw new EmpresaNotFoundException(request.Request.IdEmpresa);

            var valorCalc = _mapper.Map<ValorCalcVariavel>(request.Request)
                ?? throw new InvalidDataException("Dados inválidos.");

            await _baseRepository.AddAsync(valorCalc);
            await _baseRepository.SaveChangesAsync();


            return new CreateApiResponse { Message = "Valor Criado com sucesso" };
        }
    }
}
