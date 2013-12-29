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
    public delegate void EngineProgressChangedHandler(object source, EngineProgressChangedEventArgs e);

    #endregion

    #region Event arguments

    /// <summary>
    /// Event arguments utilized by the EngineProgressChangedHandler delegate.
    /// </summary>
    public class EngineProgressChangedEventArgs : EventArgs
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="progressPercentage"></param>
        /// <param name="metrics"></param>
        /// <param name="logFile"></param>
        public EngineProgressChangedEventArgs(int progressPercentage, EngineMetrics metrics, LogFile logFile) {
            ProgressPercentage = progressPercentage;
            Metrics = metrics;
            LogFile = logFile;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public LogFile LogFile { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public EngineMetrics Metrics { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int ProgressPercentage { get; private set; }

        #endregion
    }

    #endregion
}