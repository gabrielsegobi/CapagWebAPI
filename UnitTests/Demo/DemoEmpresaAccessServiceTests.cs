using Application.Exceptions.Empresas;
using Application.Services.Demo;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Interface;
using Moq;
using System.Linq.Expressions;

namespace UnitTests.Demo
{
    public class DemoEmpresaAccessServiceTests
    {
        private readonly Mock<ICurrentUserService> _currentUser = new();
        private readonly Mock<IBaseRepository<Empresa>> _empresaRepo = new();

        private DemoEmpresaAccessService Build() => new(_currentUser.Object, _empresaRepo.Object);

        [Fact]
        public void FilterEmpresas_sem_demo_nao_filtra()
        {
            _currentUser.Setup(c => c.IsDemo).Returns(false);
            var data = new[]
            {
                new Empresa { IdEmpresa = 1 },
                new Empresa { IdEmpresa = 2 }
            }.AsQueryable();

            var result = Build().FilterEmpresas(data).ToList();

            result.Should().HaveCount(2);
        }

        [Fact]
        public void FilterEmpresas_com_demo_filtra_por_raiz()
        {
            _currentUser.Setup(c => c.IsDemo).Returns(true);
            _currentUser.Setup(c => c.DemoCnpjRoots).Returns(new HashSet<string> { "12345678" });

            var e1 = CreateEmpresa(1, "12345678000199");
            var e2 = CreateEmpresa(2, "99999999000191");
            var data = new[] { e1, e2 }.AsQueryable();

            var result = Build().FilterEmpresas(data).ToList();

            result.Should().ContainSingle(e => e.IdEmpresa == 1);
        }

        [Fact]
        public async Task EnsureCanAccessEmpresaAsync_demo_bloqueia_cnpj_nao_permitido()
        {
            _currentUser.Setup(c => c.IsDemo).Returns(true);
            _currentUser.Setup(c => c.DemoCnpjRoots).Returns(new HashSet<string> { "12345678" });

            var empresa = CreateEmpresa(10, "99999999000191");
            _empresaRepo
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Empresa, bool>>>()))
                .ReturnsAsync(empresa);

            var act = () => Build().EnsureCanAccessEmpresaAsync(10);

            await act.Should().ThrowAsync<EmpresaDemoAccessForbiddenException>();
        }

        private static Empresa CreateEmpresa(long id, string cnpj)
        {
            var empresa = new Empresa { IdEmpresa = id };
            typeof(Empresa).GetProperty(nameof(Empresa.Cnpj))!
                .SetValue(empresa, cnpj);
            return empresa;
        }
    }
}
