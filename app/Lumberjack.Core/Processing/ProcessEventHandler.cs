using System;
using Medidata.Lumberjack.Core.Data;

namespace Medidata.Lumberjack.Core.Processing
{ 
    #region Delegates

    /// <summary>
    ///
    /// </summary>
    /// <param name="source">Object which is sending the message.</param>
    /// <param name="e">Event arguments for the event handler.</param>
    public delegate void ProcessEventHandler(object source, ProcessEventArgs e);

    #endregion

    #region Event arguments

    /// <summary>
    /// Event arguments utilized by the ProcessEventArgs delegate.
    /// </summary>
    public class ProcessEventArgs : EventArgs
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processType"></param>
        /// <param name="logFile"></param>
        /// <param name="engineMetrics"></param>
        public ProcessEventArgs(ProcessTypeEnum processType, LogFile logFile, EngineMetrics engineMetrics)
            : this(processType, logFile) {
                EngineMetrics = engineMetrics;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processType"></param>
        /// <param name="logFile"></param>
        public ProcessEventArgs(ProcessTypeEnum processType, LogFile logFile) {
            ProcessType = processType;
            LogFile = logFile;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public ProcessTypeEnum ProcessType { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public LogFile LogFile { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public EngineMetrics EngineMetrics { get; protected set; }

        #endregion
    }

    #endregion

}
