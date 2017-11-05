using System.Collections.Generic;

namespace DrHouse.Wrapper.Core.Interfaces
{
    /// <summary>
    /// Interface que define a chave principal do objeto utilizado no AutoConfig
    /// </summary>
    public interface ITablesDiagnostics
    {
        /// <summary>
        /// Lista de permissões e tabelas
        /// </summary>
        IList<ITablePermission> TablePermissions { get; }
    }
}