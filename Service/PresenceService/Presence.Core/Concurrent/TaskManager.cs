using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presence.Core.Concurrent
{
    /// <summary>
    ///-----------------------------------------------------------------
    ///   Namespace:      Presence.Core.Concurrent
    ///   Class:          TaskManager
    ///   Description:    Classe de gerenciamento das funções de execução
    ///                   de tarefas concorrentes no sistema
    ///   Author:         Leandro Piqueira                 Date: 07/11/2014
    ///   Notes:          
    ///-----------------------------------------------------------------
    ///   Revision History:
    ///   Name:            Date: //       Description: 
    ///-----------------------------------------------------------------
    /// </summary>
    public static class TaskManager
    {
        /// <summary>
        /// Executor de Tarefas - Retentor do Thread Pool da 
        /// Aplicação
        /// </summary>
        private static TaskExecutor taskExecutor = null;

        /// <summary>
        /// Retorna o executor de tarefas da aplicação
        /// </summary>
        /// <returns></returns>
        public static TaskExecutor getTaskExecutor()
        {
            if (taskExecutor == null)
                taskExecutor = new TaskExecutor();           
            return taskExecutor;
        }
    }
}
