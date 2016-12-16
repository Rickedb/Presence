using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace Presence.Core.Utils.Logger
{
    /// <summary>
    /// Parent Class of Logger, it's the Core of it.
    /// </summary>
    public abstract class EventLogger
    {
        private String logPath;
        private int logSize;
        private int totalLogs;
        private List<String> lostMessages;

        private Dictionary<string, string> logTypeXLogMessage;
        public Dictionary<string, string> LogTypeXLogMessage
        {
            get { return logTypeXLogMessage; }
            set { logTypeXLogMessage = value; }
        }

        /// <summary>
        /// Constructor for EventLogger.
        /// Advice: having a big total Logs may cause Slowness, since it'll have to rotate all logs to keep the ordenation
        /// </summary>
        /// <param name="loggerName">Log Name</param>
        /// <param name="path">Log Folder</param>
        /// <param name="logSize">Size in MB</param>
        /// <param name="totalLogs">Total logs that will be holded</param>
        public EventLogger(String loggerName, String path, int logSize = 3, int totalLogs = 9)
        {
            this.logTypeXLogMessage = new Dictionary<string, string>();
            this.logPath = path + "\\" + loggerName + "_0.log";
            this.logSize = Math.Abs(logSize) * 1000000; //Mb to Bytes
            this.totalLogs = Math.Abs(totalLogs);
            this.lostMessages = new List<String>();

            //Create first file if doesn't exists
            if (!File.Exists(this.logPath))
               File.CreateText(logPath).BaseStream.Close();
            else
                this.rotateFiles();
        }

        /// <summary>
        /// Get String depending on logType and after this log effectively on the text
        /// </summary>
        /// <param name="message">Log Message</param>
        /// <param name="logType">Type of Log</param>
        protected void logEvent(string message, EventLogType logType)
        {
            string logTypeString = string.Empty;
            switch (logType)
            {
                case EventLogType.ERROR:
                    logTypeString = " - [ERROR]: ";
                    break;
                case EventLogType.WARNING:
                    logTypeString = " - [WARNING]: ";
                    break;
                case EventLogType.INFORMATION:
                default:
                    logTypeString = " - [INFO]: ";
                    break;
            }
            this.logEvent(message, logTypeString);
        }

        /// <summary>
        /// Method that effectively log the message to the archive
        /// </summary>
        /// <param name="message">Log Message</param>
        /// <param name="logHeaderTypeString">Log Header</param>
        protected void logEvent(string message, string logHeaderTypeString)
        {
            try
            {
                message = message.Replace("\n", "\n" + string.Empty.PadLeft(28 + logHeaderTypeString.Length)).Trim(); //Align

                message = "[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + "] ->" + logHeaderTypeString + message;

                using (StreamWriter currentFile = new StreamWriter(this.logPath, true))
                {
                    if (lostMessages.Count > 0)
                    {
                        foreach (String msg in lostMessages)
                            currentFile.WriteLine(msg);
                        lostMessages.Clear();
                    }
                    
                    currentFile.WriteLine(message);
                }
            }
            catch (IOException)
            {
                this.lostMessages.Add(message);
            }
            catch (Exception ex)
            {
                try
                {
                    throw ex;
                }
                catch (Exception)
                {

                }
            }

            this.rotateFiles();
        }

        /// <summary>
        /// Create a new text file when the actual reaches the logSize(in MB)
        /// </summary>
        private void rotateFiles()
        {
            FileInfo file = new FileInfo(logPath);
            if (file.Length > logSize)
            {
                FileInfo rotatingFiles;
                for (int i = totalLogs; i >= 0; i--)
                {
                    rotatingFiles = new FileInfo(this.logPath.Replace("_0.log", "_" + i + ".log"));
                    if (i == totalLogs)
                    {
                        if (rotatingFiles.Exists)
                            rotatingFiles.Delete();
                    }
                    else
                    {
                        if (rotatingFiles.Exists)
                            File.Move(this.logPath.Replace("_0.log", "_" + i + ".log"), this.logPath.Replace("_0.log", "_" + (i + 1) + ".log"));
                    }
                }
                File.CreateText(logPath).Close();
            }
        }

        /// <summary>
        /// Event Log Type
        /// </summary>
        protected enum EventLogType
        {
            ERROR,
            WARNING,
            INFORMATION,
            CUSTOM
        }
    }
}
