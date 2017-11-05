using DrHouse.Wrapper.Core;
using DrHouse.Wrapper.Core.Interfaces;
using DrHouse.Wrapper.Repository;
using Nerdle.AutoConfig;
using System.Linq;
using Xunit;

namespace DrHouse.Wrapper.Tests
{
    public class HealthCheckTest
    {
        [Fact]
        public void AppConfig_Return_AllObjects()
        {
            // Usa o autoConfig para ler uma seção do web.config com todas as tabelas e permissões
            var tablesDiagnostics = AutoConfig.Map<ITablesDiagnostics>();

            // Quantidade de registros no app.config
            Assert.True(tablesDiagnostics.TablePermissions.Count == 3);
        }

        [Fact]
        public async void HealthCheck_Return_WithError()
        {
            // Usa o autoConfig para ler uma seção do web.config com todas as tabelas e permissões
            var tablesDiagnostics = AutoConfig.Map<ITablesDiagnostics>();

            // Repositório para validar a saúde
            var heathCheckRepository = new HeathCheckRepository("DrHouse.Wrapper", "BdName");

            var healthCheck = new HealthCheck(heathCheckRepository);
            var result = await healthCheck.HealthCheckAsync(tablesDiagnostics);

            Assert.False(result.First().IsSuccess);
        }
    }
}