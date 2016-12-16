using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Presence.Core.Concurrent
{
    /// <summary>
    ///-----------------------------------------------------------------
    ///   Namespace:      Presence.Core.Concurrent
    ///   Class:          TaskExecutor
    ///   Description:    Classe gerenciadora do Pool de Threads desta aplicação
    ///   Author:         Leandro Piqueira                 Date: 07/11/2014
    ///   Notes:          
    ///-----------------------------------------------------------------
    ///   Revision History:
    ///   Name:            Date: //       Description: 
    ///-----------------------------------------------------------------
    /// </summary>
    public class TaskExecutor
    {
        /// <summary>
        /// Lista endereçada de Trabalhadores do Sistema
        /// </summary>
        private Dictionary<String, AbstractWorker> workers;

        public Dictionary<String, AbstractWorker> Workers
        {
            get { return workers; }
        }

        /// <summary>
        /// Cria uma instância do executor de tarefas
        /// </summary>
        public TaskExecutor()
        {
            this.workers = new Dictionary<String, AbstractWorker>();
        }

        /// <summary>
        /// Inicia um trabalhador o colocando para execução no
        /// Pool de Threads
        /// </summary>
        /// <param name="worker">Trabalhador pronto para execução da tarefa</param>
        public void executeTask(AbstractWorker worker)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(worker.run));
            this.workers.Add(worker.Name, worker);
        }

        /// <summary>
        /// Finaliza a execução de um Trabalhador específico
        /// </summary>
        /// <param name="workerName"></param>
        public void finalizeTask(String workerName)
        {
            this.workers[workerName].finalize();
            this.workers.Remove(workerName);
        }

        /// <summary>
        /// Finaliza todas as Threads Trabalhadoras do sistema
        /// </summary>
        public void finalizeAll()
        {
            ICollection<String> keys = this.workers.Keys;
            foreach (String workerName in keys)
            {
                this.workers[workerName].finalize();
            }
            this.workers.Clear();
        }
    }
}
