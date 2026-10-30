using Application.Exceptions.Empresas;
using Application.Helpers;
using Domain.Entities;
using Infrastructure.Interface;

namespace Application.Services.Demo
{
    public class DemoEmpresaAccessService : IDemoEmpresaAccessService
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IBaseRepository<Empresa> _empresaRepository;

        public DemoEmpresaAccessService(
            ICurrentUserService currentUser,
            IBaseRepository<Empresa> empresaRepository)
        {
            _currentUser = currentUser;
            _empresaRepository = empresaRepository;
        }

        public bool IsDemoUser => _currentUser.IsDemo;

        public IQueryable<Empresa> FilterEmpresas(IQueryable<Empresa> query)
        {
            if (!_currentUser.IsDemo)
                return query;

            var roots = _currentUser.DemoCnpjRoots;
            if (roots.Count == 0)
                return query.Where(_ => false);

            var rootList = roots.ToList();
            return query.Where(e =>
                e.Cnpj.Length >= 8 && rootList.Contains(e.Cnpj.Substring(0, 8)));
        }

        public bool CanAccessCnpj(string? cnpj)
        {
            if (!_currentUser.IsDemo)
                return true;

            return CnpjRootHelper.MatchesRoot(cnpj, _currentUser.DemoCnpjRoots);
        }

        public async Task EnsureCanAccessEmpresaAsync(long idEmpresa, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsDemo)
                return;

            var empresa = await _empresaRepository.GetFirstOrDefaultAsync(e => e.IdEmpresa == idEmpresa);
            if (empresa == null)
                throw new EmpresaNotFoundException(idEmpresa);

            if (!CanAccessCnpj(empresa.Cnpj))
                throw new EmpresaDemoAccessForbiddenException();
        }
    }
}
