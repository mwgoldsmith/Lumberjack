using System;

namespace Medidata.Lumberjack.Core.Data.Collections
{
    #region Delegates

    /// <summary>
    ///
    /// </summary>
    /// <param name="source">Object which is sending the message.</param>
    /// <param name="e">Event arguments for the event handler.</param>
    public delegate void LogFileUpdatedHandler(object source, LogFilesUpdatedEventArgs e);

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public class LogFilesUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFiles"></param>
        public LogFilesUpdatedEventArgs(LogFile[] logFiles) {
            LogFiles = logFiles;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        public LogFilesUpdatedEventArgs(LogFile logFile)
            : this(new[] {logFile}) {
        }

        /// <summary>
        /// 
        /// </summary>
        public LogFile[] LogFiles { get; private set; }

    }
}
