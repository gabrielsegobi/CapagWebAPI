using Application.Commands.DescricaoDebitos;
using Application.Exceptions.Empresas;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.DescricaoDebitos
{
    public class CreateDescricaoDebitoHandler : IRequestHandler<CreateDescricaoDebitoCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<DescricaoDebito> _baseRepository;
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IMapper _mapper;

        public CreateDescricaoDebitoHandler(IBaseRepository<DescricaoDebito> baseRepository, IBaseRepository<Empresa> empresaRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _empresaRepository = empresaRepository;
            _mapper = mapper;
        }

        public async Task<CreateApiResponse> Handle(CreateDescricaoDebitoCommand request, CancellationToken cancellationToken)
        {
            var debito = _mapper.Map<DescricaoDebito>(request.Request)
                ?? throw new InvalidDataException("Invalid data");

            _ = await _empresaRepository.GetByIdAsync(request.Request.IdEmpresa)
                ?? throw new EmpresaNotFoundException(request.Request.IdEmpresa);

            await _baseRepository.AddAsync(debito);
            await _baseRepository.SaveChangesAsync();



            return new CreateApiResponse("Débito Criado com sucesso", debito.IdDescricaoDebitos);
        }
    }
}
