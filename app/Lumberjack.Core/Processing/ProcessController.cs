using System;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Logging;

namespace Medidata.Lumberjack.Core.Processing
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ProcessController : SessionObject, IDisposable
    {
        #region Initializers
        
        /// <summary>
        /// 
        /// </summary>
        public ProcessController() : this(null) { }

        /// <summary>
        /// 
        /// </summary>
        public ProcessController(UserSession session) : base(session) {
            HashingEngine = new HashingEngine(session);
            HashingEngine.ProgressChanged += HashingEngine_ProgressChanged;
            HashingEngine.Completed += Engine_Completed;

            FileParsingEngine = new FileParsingEngine(session);
            FileParsingEngine.ProgressChanged += FileParsingEngine_ProgressChanged;
            FileParsingEngine.Completed += Engine_Completed;

            EntryParsingEngine = new EntryParsingEngine(session);
            EntryParsingEngine.ProgressChanged += EntryParsingEngine_ProgressChanged;
            EntryParsingEngine.Completed += Engine_Completed;
        }
        
        #endregion

        #region Event handlers

        /// <summary>
        /// 
        /// </summary>
        public event ProcessEventHandler ProgressChanged;

        /// <summary>
        /// 
        /// </summary>
        public event ProcessEventHandler LogCompleted;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public HashingEngine HashingEngine { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public FileParsingEngine FileParsingEngine { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public EntryParsingEngine EntryParsingEngine { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        public bool IsRunning {
            get { return HashingEngine.IsRunning || FileParsingEngine.IsRunning || EntryParsingEngine.IsRunning; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        public void Start() {
            var logger = Logger.GetInstance();

            logger.Trace("PC-START-001");

            if (SessionInstance.LogFiles.Any(l => l.HashStatus == EngineStatusEnum.None) && !HashingEngine.IsRunning) {
                logger.Trace("PC-START-002a");
                if (HashingEngine.Start()) 
                    logger.Trace("PC-START-002b - running");
            }

            if (SessionInstance.LogFiles.Any(l => l.FilenameParseStatus == EngineStatusEnum.None) && !FileParsingEngine.IsRunning) {
                logger.Trace("PC-START-003a");
                if (FileParsingEngine.Start()) 
                    logger.Trace("PC-START-003b - running");
            }

            if (SessionInstance.LogFiles.Any(l => l.EntryParseStatus == EngineStatusEnum.None && l.FilenameParseStatus == EngineStatusEnum.Completed)
                && !EntryParsingEngine.IsRunning) {
                logger.Trace("PC-START-004a");
                if (EntryParsingEngine.Start()) 
                    logger.Trace("PC-START-004b - running");
            }

            logger.Trace("PC-START-009");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void Stop() {
            if (HashingEngine.IsRunning)
                HashingEngine.Stop();
            if (FileParsingEngine.IsRunning)
                FileParsingEngine.Stop();
            if (EntryParsingEngine.IsRunning)
                EntryParsingEngine.Stop();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void Engine_Completed(object source, EngineCompletedEventArgs e) {
            var engine = (EngineBase)source;

            if (e.Cancelled) {
                OnInfo(String.Format("Engine {0} has been cancelled.", engine.Name));
            } else if (e.Error != null) {
                OnError(String.Format("Engine {0} failed with exception(s).", engine.Name), e.Error);
                Stop();
            } else {
                OnInfo(String.Format("Engine {0} has completed processing.", engine.Name));
                Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void HashingEngine_ProgressChanged(object source, EngineProgressChangedEventArgs e) {
            var logFile = e.LogFile;

            if (logFile.HashStatus == EngineStatusEnum.Completed) {
                OnInfo(String.Format("File \"{0}\" has been hashed ({1} bytes, MD5 hash: {2})", logFile.Filename, logFile.Filesize, logFile.Md5Hash));
                OnLogCompleted(source, ProcessTypeEnum.Hash, logFile, e.Metrics);
            } else {
                if (logFile.HashStatus != EngineStatusEnum.Processing)
                    OnError(String.Format("Failed to determine hash of file \"{0}\".", logFile.Filename));

                OnProgressChanged(source, ProcessTypeEnum.Hash, logFile, e.Metrics);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void FileParsingEngine_ProgressChanged(object source, EngineProgressChangedEventArgs e) {
            var logFile = e.LogFile;

            if (logFile.FilenameParseStatus == EngineStatusEnum.Completed) {
                OnInfo(String.Format("Format of log file \"{0}\" is \"{1}\".", logFile.Filename, logFile.SessionFormat.Reference));
                OnLogCompleted(source, ProcessTypeEnum.Filename, logFile, e.Metrics);
            } else {
                if (logFile.FilenameParseStatus != EngineStatusEnum.Processing)
                    OnError(String.Format("Failed to process filename for log \"{0}\".", logFile.Filename));

                OnProgressChanged(source, ProcessTypeEnum.Filename, logFile, e.Metrics);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void EntryParsingEngine_ProgressChanged(object source, EngineProgressChangedEventArgs e) {
            var logFile = e.LogFile;

            if (logFile.EntryParseStatus == EngineStatusEnum.Completed) {
                OnInfo(String.Format("Log entries for \"{0}\" have been parsed.", logFile.Filename));
                OnLogCompleted(source, ProcessTypeEnum.Entries, logFile, e.Metrics);
            } else {
                if (logFile.EntryParseStatus != EngineStatusEnum.Processing)
                    OnError(String.Format("Failed to parse log entries for \"{0}\".", logFile.Filename));

                OnProgressChanged(source, ProcessTypeEnum.Entries, logFile, e.Metrics);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="processType"></param>
        /// <param name="logFile"></param>
        /// <param name="engineMetrics"></param>
        private void OnLogCompleted(object source, ProcessTypeEnum processType, LogFile logFile, EngineMetrics engineMetrics) {
            if (LogCompleted != null)
                LogCompleted.Invoke(source, new ProcessEventArgs(processType, logFile, engineMetrics));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="processType"></param>
        /// <param name="logFile"></param>
        /// <param name="engineMetrics"></param>
        private void OnProgressChanged(object source, ProcessTypeEnum processType, LogFile logFile, EngineMetrics engineMetrics) {
            if (ProgressChanged != null)
                ProgressChanged.Invoke(source, new ProcessEventArgs(processType, logFile, engineMetrics));
        }
        
        #endregion

        #region IDisposable implementation

        /// <summary>
        /// 
        /// </summary>
        private bool IsDisposed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isDisposing"></param>
        private void Dispose(bool isDisposing) {
            try {
                if (IsDisposed)
                    return;

                if (isDisposing) {
                    if (HashingEngine != null) {
                        HashingEngine.Dispose();
                    }
                    if (FileParsingEngine != null) {
                        FileParsingEngine.Dispose();
                    }
                    if (EntryParsingEngine != null) {
                        EntryParsingEngine.Dispose();
                    }
                }

                HashingEngine = null;
                FileParsingEngine = null;
                EntryParsingEngine = null;
            } finally {
                IsDisposed = true;
            }
        }

        #endregion
    }
}
