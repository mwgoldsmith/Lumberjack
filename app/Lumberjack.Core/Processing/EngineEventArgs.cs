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
    public delegate void EngineHandler(object source, EngineEventArgs e);

    #endregion

    #region Event arguments

    /// <summary>
    /// Event arguments utilized by the LogFileEventArgs delegate.
    /// </summary>
    public class EngineEventArgs : EventArgs
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="engineAction"></param>
        /// <param name="logFile"></param>
        /// <param name="engineMetrics"></param>
        public EngineEventArgs(ProcessTypeEnum engineAction, LogFile logFile, EngineMetrics engineMetrics)
            : this(engineAction, logFile) {
                EngineMetrics = engineMetrics;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="engineAction"></param>
        /// <param name="logFile"></param>
        public EngineEventArgs(ProcessTypeEnum engineAction, LogFile logFile) {
            EngineAction = engineAction;
            LogFile = logFile;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public ProcessTypeEnum EngineAction { get; protected set; }

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
