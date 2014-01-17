using System;
using System.IO;
using System.Text;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Data.Formats;

namespace Medidata.Lumberjack.Core.Processing
{
    public class EngineState
    {
        #region Private fields

        private LogFile _logFile;

        private StreamReader _sr;

        #endregion

        #region Initializers

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public LogFile LogFile {
            get { return _logFile; }
            set {
                if (_logFile == value)
                    return;

                _logFile = value;

                CurrentFormat = _logFile.SessionFormat;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public StreamReader StreamReader {
            get { return _sr; }
            set {
                if (_sr == value)
                    return;

                _sr = value;

                Encoding = StreamReader.CurrentEncoding;
                TotalBytes = _sr.BaseStream.Length;
                BytesRead = 0;
                BytesProcessed = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public StreamWriter StreamWriter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SessionFormat CurrentFormat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FormatContextEnum CurrentContext { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long TotalBytes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long BytesRead { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long BytesProcessed { get; set; }

        public DateTime LastUpdate { get; set; }

        #endregion
    }
}