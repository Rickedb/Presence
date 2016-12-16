using Presence.Core.Concurrent;
using Presence.Core.SystemCore;
using Presence.Service.Tasks;
using SimpleInjector;
using System;


namespace Presence.Service
{
    public class PresenceServiceInitializer 
    {
        public PresenceServiceInitializer(Container container)
        {
            ServiceCore.configure(container);
            MainTask.configure();
        }

        /// <summary>
        /// Instância única da classe executora do sistemas
        /// </summary>
        private static PresenceServiceInitializer instance = null;

        /// <summary>
        /// Obtém a instância unica do Sistema
        /// </summary>
        /// <returns></returns>
        public static PresenceServiceInitializer getInstance()
        {
            if (instance != null)
                return instance;

            throw new NullReferenceException("Configure a aplicação antes de sua execução! Chame o método configure passando os devidos parâmetros!");
        }

        /// <summary>
        /// Método de configuração do Sistema. Primeiro método
        /// que deve ser executado para configuração e inicialização
        /// do Sistema
        /// </summary>
        /// <param name="parameters">Parâmetros do Sistema</param>
        public static void configure(Container container)
        {
            instance = new PresenceServiceInitializer(container);
        }

        /// <summary>
        /// Stop MainTask and all tasks below it
        /// </summary>
        public void Stop()
        {
            TaskManager.getTaskExecutor().finalizeAll();
        }
    }
}
