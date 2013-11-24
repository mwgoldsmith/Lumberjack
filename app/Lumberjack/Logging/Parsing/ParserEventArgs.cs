using System;

namespace Medidata.Lumberjack.Logging.Parsing
{
    #region Delegates

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public delegate void ParserEventHandler(object source, ParserEventArgs e);

    #endregion

    public class ParserEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="curParsedLog"></param>
        public ParserEventArgs(Log curParsedLog)
        {
            CurParsedLog = curParsedLog;
        }

        /// <summary>
        /// 
        /// </summary>
        public Log CurParsedLog { get; private set; }
        
        public int TotalLogs { get; set; }

        public int CurrentLogIndex { get; set; }
    }
}
