using System;

namespace Presence.Core.Utils.Logger
{
    /// <summary>
    /// Class Logger responsible for getting message and send to abstract class
    /// </summary>
    public class Logger : EventLogger
    {

        /// <summary>
        /// Create a logger instance
        /// </summary>
        /// <param name="logName">File Name</param>
        /// <param name="path">File Folder</param>
        public Logger(String logName, String path) : base(logName, path)
        {
            
        }

        /// <summary>
        /// Create a logger instance
        /// </summary>
        /// <param name="logName">File Name</param>
        /// <param name="path">File Folder</param>
        /// <param name="logSize">File Size (in MB)</param>
        public Logger(String logName, String path, int logSize)
            : base(logName, path, logSize)
        {

        }

        /// <summary>
        /// Create a logger instance
        /// </summary>
        /// <param name="logName">File Name</param>
        /// <param name="path">File Folder</param>
        /// <param name="logSize">File Size (in MB)</param>
        /// <param name="totalLogs">Total Files to Hold</param>
        public Logger(String logName, String path, int logSize, int totalLogs)
            : base(logName, path, logSize, totalLogs)
        {

        }

        /// <summary>
        /// Log as an Information type
        /// </summary>
        /// <param name="message">Log Message</param>
        public void log(String message)
        {
            this.logEvent(message, EventLogType.INFORMATION);
        }

        /// <summary>
        /// Log as an Error type
        /// </summary>
        /// <param name="message">Log Message</param>
        public void logError(String message)
        {
            this.logEvent(message, EventLogType.ERROR);
        }

        /// <summary>
        /// Log as a Warning Type
        /// </summary>
        /// <param name="message">Log Message</param>
        public void logWarning(String message)
        {
            this.logEvent(message, EventLogType.WARNING);
        }

        /// <summary>
        /// Log a message with the custom type defined by user
        /// </summary>
        /// <param name="message"></param>
        public void logCustomType(String message, String customLogTypeName)
        {
            if (this.LogTypeXLogMessage.ContainsKey(customLogTypeName))
                this.logEvent(message, this.LogTypeXLogMessage[customLogTypeName]);
        }

        /// <summary>
        /// Create a new custom type
        /// </summary>
        /// <param name="logTypeName">Log Type Name</param>
        /// <param name="logHeaderMessage">Log header Ex: [(MyCustomHeader)]: (message)</param>
        public void createCustomTypeLog(String logTypeName, String logHeaderMessage)
        {
            this.LogTypeXLogMessage.Add(logTypeName, " - [" + logHeaderMessage.ToUpper() + "]: ");
        }
    }
}
