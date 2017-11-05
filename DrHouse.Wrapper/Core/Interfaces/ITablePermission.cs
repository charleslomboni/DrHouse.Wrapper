namespace DrHouse.Wrapper.Core.Interfaces
{
    /// <summary>
    /// Interface principal que fornecem nome e permissão da tabela passada
    /// </summary>
    public interface ITablePermission
    {
        /// <summary>
        /// Noma da tabela
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Tipo de permissão da tabela
        /// </summary>
        string Permissions { get; }
    }
}