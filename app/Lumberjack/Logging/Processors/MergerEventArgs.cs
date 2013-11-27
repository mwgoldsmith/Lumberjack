using System;

namespace Medidata.Lumberjack.Logging.Processors
{
    public enum MergerAction
    {
        CalculatedTimeSpan,
        CountedEntries,
        Indexing,
        Joining,
        Completed,
        Failed,
        Stopped
    }

    #region Delegates

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public delegate void MergerEventHandler(object source, MergerEventArgs e);

    #endregion

    public class MergerEventArgs : EventArgs
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="message"></param>
        /// <param name="param"></param>
        public MergerEventArgs(MergerAction action, string message, object param)
        {
            Action = action;
            Param = param;
            Message = message;
        }

        /// <summary>
        /// 
        /// </summary>
        public MergerAction Action { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public object Param { get; private set; }
    }
}
