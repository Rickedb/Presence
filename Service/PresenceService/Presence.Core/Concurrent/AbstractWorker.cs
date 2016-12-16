using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Presence.Core.Utils.Logger;
using Presence.Core.SystemCore;

namespace Presence.Core.Concurrent
{
    /// <summary>
    ///-----------------------------------------------------------------
    ///   Namespace:      Presence.Core.Concurrent
    ///   Class:          AbstractWorker
    ///   Description:    Classe abstrata que representa uma tarefa trabalhadora
    ///                   da aplicação
    ///   Author:         Leandro Piqueira                 Date: 07/11/2014
    ///   Notes:          
    ///-----------------------------------------------------------------
    ///   Revision History:
    ///   Name:            Date: //       Description: 
    ///-----------------------------------------------------------------
    /// </summary>
    public abstract class AbstractWorker
    {
        /// <summary>
        /// Nome de Identificação do Trabalhador
        /// </summary>
        private String name;

        /// <summary>
        /// Controle de ativação/desativação da tarefa
        /// </summary>
        private bool active = false;

        /// <summary>
        /// Gerenciador de Logs da aplicação
        /// </summary>
        private Logger logger;

        /// <summary>
        /// Intervalo de execução da thread
        /// </summary>
        private int executeInterval;

        /// <summary>
        /// Assinatura para implementação da rotina de inicialização.
        /// Esta rotina é executada apenas uma vez no inicio da execução
        /// da Thread - pode ser utilizada para inicializações longas e 
        /// pesadas que podem ser executadas diretamente na Thread (depois do contrutor)
        /// </summary>
        public abstract void initialize();

        /// <summary>
        /// Assinatura para implementação do método de execução das tarefas do
        /// Trabalhador. Método executado periodicamente de acordo com parâmetro
        /// executeInterval que define a periodicidade das chamadas a este método.
        /// </summary>
        public abstract void executeTask();
        
        /// <summary>
        /// Cria uma Worker informando o nome da tarefa e o intervalo de 
        /// execução das tarefas do trabalhador
        /// </summary>
        /// <param name="name"></param>
        /// <param name="executeInterval"></param>
        public AbstractWorker(String name, int executeInterval)
        {
            this.name = name;
            this.executeInterval = executeInterval;
        }
        
        /// <summary>
        /// Rotina principal que será lançada a execução pelo Pool de Threads
        /// em uma Thread específica
        /// </summary>
        public void run(object state)
        {
            this.logger = new Logger(name, this.getServiceCore().ServiceProperties.LogPath);
            this.initialize();
            this.active = true;
            this.logger.log("Iniciando execução da tarefa!");
            while (this.active)
            {
                try
                {
                    this.executeTask();
                    Thread.Sleep(executeInterval);
                }
                catch (Exception ex)
                {
                    this.logger.logError("FATAL ERROR INSIDE THREAD, EXCEPTION DATA:" +
                                         "\nException Message: " + ex.Message +
                                         "\nStack Trace: " + ex.StackTrace +
                                         "\nInner Exception: " + ex.InnerException);
                    this.active = false;
                }
            }
            this.logger.log("Tarefa finalizada!");
        }

        /// <summary>
        /// Finaliza a execução deste trabalhador
        /// </summary>
        public virtual void finalize()
        {
            this.logger.log("Finalizando execução da tarefa!");
            this.active = false;
        }

        /// <summary>
        /// Retorna o Nome identificador do Trabalhador
        /// </summary>
        public String Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Retorna o status de ativação do Trabalhador
        /// </summary>
        public bool Active
        {
            get { return this.active; }
        }

        /// <summary>
        /// Retorna o gerenciador de Logs da Thread
        /// </summary>
        /// <returns>Gerenciador de logs</returns>
        public Logger getLogger()
        {
            return this.logger;
        }

        /// <summary>
        /// Returns Service Core
        /// </summary>
        /// <returns>Gerenciador de logs</returns>
        public ServiceCore getServiceCore()
        {
            return ServiceCore.getInstance();
        }

    }
}
