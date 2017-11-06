using DrHouse.Core;
using DrHouse.SqlServer;
using DrHouse.Wrapper.Core.Interfaces;
using DrHouse.Wrapper.Domain;
using DrHouse.Wrapper.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrHouse.Wrapper.Repository
{
    /// <summary>
    /// Check de validação para as tabelas do repositório
    /// </summary>
    public class HeathCheckRepository : IHeathCheckRepository
    {
        /// <summary>
        /// Dependências do banco
        /// </summary>
        private readonly SqlServerHealthDependency _dbDependency;

        /// <summary>
        /// Objeto principal para check
        /// </summary>
        private readonly HealthChecker _healthChecker;

        /// <summary>
        /// Construtor default
        /// </summary>
        /// <param name="applicationName">Nome da aplicação que está usando o DrHouse</param>
        /// <param name="connectionString">String de conexão para healthCheck</param>
        public HeathCheckRepository(string applicationName, string connectionString)
        {
            // Passa o nome da aplicação para o nuget
            _healthChecker = new HealthChecker(applicationName);

            // Passa o nome da connectionString para o nuget
            _dbDependency = new SqlServerHealthDependency(connectionString);

            // Adiciona a dependência ao DrHouse
            _healthChecker.AddDependency(_dbDependency);
        }

        /// <summary>
        /// Valida a permissão das tabelas necessárias para o funcionamento do ptolemeu
        /// </summary>
        /// <param name="tablesDiagnostics"></param>
        /// <returns></returns>
        public async Task<IList<Diagnostic>> CheckTablePermissionsAsync(ITablesDiagnostics tablesDiagnostics)
        {
            foreach (var table in tablesDiagnostics.TablePermissions)
            {
                // Recupera as permissões da tabela passada
                var permissions = GetPermissions(table);

                // Adiciona as tabelas e as permissões necessárias para o funcionamento
                _dbDependency.AddTableDependency(table.Name, permissions);
            }

            // Executa o healthCheck
            var healthData = await Task.Run(() => _healthChecker.CheckHealth()).ConfigureAwait(false);

            // Constrói uma lista de Diagnostico
            var buildHealthData = BuildHealthData(healthData);

            return buildHealthData;
        }

        /// <summary>
        /// Constrói uma lista de Diagnóstico
        /// </summary>
        /// <param name="healthData"></param>
        /// <returns></returns>
        private static IList<Diagnostic> BuildHealthData(HealthData healthData)
        {
            var buildHealthData = healthData.DependenciesStatus.Where(data => !data.IsOK)
                .Select(data => new Diagnostic
                {
                    FlowName = "DatabasePermission",
                    IsSuccess = healthData.IsOK,
                    Data = GetErrorMessages(healthData)
                }).ToList();
            return buildHealthData;
        }

        /// <summary>
        /// Recupera as permissões baseada do objeto recebido
        /// </summary>
        /// <param name="tablePermissions">Lista de permissões</param>
        /// <returns></returns>
        private static Permission GetPermissions(ITablePermission tablePermissions)
        {
            // Separa as permissões separadas por ; no config
            var permissions = SplitPermissionsValue(tablePermissions);

            // Default valor
            var result = Permission.Undefined;

            if (HasAllPermissions(permissions))
            {
                result = Permission.DELETE | Permission.SELECT | Permission.UPDATE | Permission.INSERT;
                return result;
            }

            foreach (var permission in permissions)
            {
                var permissionParsed = Permission.Undefined;

                var canParse = Enum.TryParse(permission.ToUpper(), out permissionParsed);
                if (!canParse)
                    throw new ApplicationException("There is a permissions config that was not correctly setted: " +
                                                   $"Table: {tablePermissions.Name}; Permission:{permission}");
                result |= permissionParsed;
            }
            return result;
        }

        /// <summary>
        /// Verifica se tem todas as permissões do CRUD
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        private static bool HasAllPermissions(IReadOnlyCollection<string> permissions)
        {
            var hasOnlyOnePermission = permissions.Count == 1;
            var firstPermission = permissions.First();

            return hasOnlyOnePermission && firstPermission.Equals("all", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Sepera as informações recebidas
        /// </summary>
        /// <param name="tablePermissions"></param>
        /// <returns></returns>
        private static string[] SplitPermissionsValue(ITablePermission tablePermissions)
        {
            var permissionsString = tablePermissions.Permissions;
            var permissions = permissionsString.Split(';');
            return permissions;
        }

        /// <summary>
        /// Monta o objeto HeathData com a mensagem de erro
        /// </summary>
        /// <param name="data"></param>
        /// <param name="healthData"></param>
        /// <returns></returns>
        private static HealthData GetErrorMessages(HealthData data, HealthData healthData = null)
        {
            if (healthData == null)
            {
                healthData = new HealthData(data.Name);
            }

            if (!data.IsOK)
            {
                healthData.DependenciesStatus.Add(new HealthData(data.Name)
                {
                    IsOK = false,
                    ErrorMessage = data.ErrorMessage,
                    Type = data.Type
                });
                foreach (var dependency in data.DependenciesStatus)
                {
                    GetErrorMessages(dependency, healthData);
                }
            }
            return healthData;
        }
    }
}