using DrHouse.Wrapper.Core.Interfaces;
using DrHouse.Wrapper.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrHouse.Wrapper.Repository.Interfaces
{
    /// <summary>
    /// Repositório para diagnóstico da saúde da conexão com o banco de dados
    /// </summary>
    public interface IHeathCheckRepository
    {
        /// <summary>
        /// Valida a permissão das tabelas necessárias para o funcionamento do ptolemeu
        /// </summary>
        /// <param name="tablesDiagnostics"></param>
        /// <returns></returns>
        Task<IList<Diagnostic>> CheckTablePermissionsAsync(ITablesDiagnostics tablesDiagnostics);
    }
}