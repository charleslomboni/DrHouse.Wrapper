using DrHouse.Wrapper.Core.Interfaces;
using DrHouse.Wrapper.Domain;
using DrHouse.Wrapper.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DrHouse.Wrapper.Core
{
    /// <summary>
    /// Checks de validação para saúde da aplicação
    /// </summary>
    public class HealthCheck : IHealthCheck
    {
        private readonly IHeathCheckRepository _heathCheckRepository;

        public HealthCheck(IHeathCheckRepository heathCheckRepository)
        {
            _heathCheckRepository = heathCheckRepository;
        }

        /// <summary>
        /// Valida as permissões das tabelas passadas
        /// </summary>
        /// <param name="tablesDiagnostics"></param>
        /// <returns></returns>
        public async Task<IList<Diagnostic>> HealthCheckAsync(ITablesDiagnostics tablesDiagnostics)
        {
            var diagnosticResults = new List<Diagnostic>();

            var checksTablesResult = await _heathCheckRepository.CheckTablePermissionsAsync(tablesDiagnostics);

            foreach (var databaseDiagnostic in checksTablesResult)
            {
                var dataBasePermissionDiagnostic = await Check(async () => databaseDiagnostic).ConfigureAwait(false);
                diagnosticResults.Add(dataBasePermissionDiagnostic);
            }

            return diagnosticResults;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="diagnosticAction"></param>
        /// <returns></returns>
        private static async Task<Diagnostic> Check(Func<Task<Diagnostic>> diagnosticAction)
        {
            var timeWatcher = Stopwatch.StartNew();

            var diagnosticResult = await diagnosticAction().ConfigureAwait(false);
            diagnosticResult.ElapsedTime = timeWatcher.Elapsed;
            diagnosticResult.Data = diagnosticResult.Data;

            timeWatcher.Stop();
            return diagnosticResult;
        }
    }
}