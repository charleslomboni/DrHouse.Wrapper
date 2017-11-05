using DrHouse.Wrapper.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrHouse.Wrapper.Core.Interfaces
{
    /// <summary>
    /// Checks de validação para saúde da aplicação
    /// </summary>
    public interface IHealthCheck
    {
        /// <summary>
        /// Valida as permissões das tabelas passadas
        /// </summary>
        /// <param name="tablesDiagnostics"></param>
        /// <returns></returns>
        Task<IList<Diagnostic>> HealthCheckAsync(ITablesDiagnostics tablesDiagnostics);
    }
}