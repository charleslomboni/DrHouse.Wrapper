using DrHouse.Core;
using System;

namespace DrHouse.Wrapper.Domain
{
    /// <summary>
    /// Dados de diagnóstico do sistema
    /// </summary>
    public class Diagnostic
    {
        public string FlowName { get; set; }

        /// <summary>
        /// Se o diagnóstico foi bem sucedido
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Mensagem sobre o diagnóstico
        /// </summary>
        public HealthData Data { get; set; }

        /// <summary>
        /// Tempo levado para a execução do diagnótico em questão
        /// </summary>
        public TimeSpan ElapsedTime { get; set; }
    }
}