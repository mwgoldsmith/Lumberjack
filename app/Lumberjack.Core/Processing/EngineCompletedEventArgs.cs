using System;

namespace Medidata.Lumberjack.Core.Processing
{
    #region Delegates

    /// <summary>
    ///
    /// </summary>
    /// <param name="source">Object which is sending the message.</param>
    /// <param name="e">Event arguments for the event handler.</param>
    public delegate void EngineCompletedHandler(object source, EngineCompletedEventArgs e);

    #endregion

    #region Event arguments

    /// <summary>
    /// Event arguments utilized by the EngineCompletedHandler delegate.
    /// </summary>
    public class EngineCompletedEventArgs : EventArgs
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <param name="cancelled"></param>
        /// <param name="metrics"></param>
        public EngineCompletedEventArgs(Exception error, bool cancelled, EngineMetrics metrics) {
            Cancelled = cancelled;
            Error = error;
            Metrics = metrics;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool Cancelled { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public EngineMetrics Metrics { get; private set; }

        #endregion
    }

    #endregion
}
