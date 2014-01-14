using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Logging;

namespace Medidata.Lumberjack.Core.Processing
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class EngineBase : SessionObject, IDisposable
    {
        #region Constants

        protected const string ContentField = "CONTENT";

        #endregion

        #region Private fields

        protected static readonly object _locker = new object();

        private volatile bool _isRunning;
        private volatile bool _isStopping;

        private Thread _thread;
        private Logger _logger;
        private Task _task;

        #endregion

        #region Initializers
        
        /// <summary>
        /// 
        /// </summary>
        protected EngineBase() : this(null) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        protected EngineBase(UserSession session) : base(session) {
            _logger = Logger.GetInstance();
            Name = GetType().Name;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// 
        /// </summary>
        public event EngineProgressChangedHandler ProgressChanged;

        /// <summary>
        /// 
        /// </summary>
        public event EngineCompletedHandler Completed;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool IsRunning {
            get { return _isRunning; }
            protected set { _isRunning = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsStopping {
            get { return _isStopping; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected Logger Logger {
            get { return _logger ?? (_logger = Logger.GetInstance()); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Start() {
            return Start(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metrics"></param>
        /// <returns></returns>
        public bool Start(EngineMetrics metrics) {
            if (Logger.IsTraceEnabled)
                Logger.Trace("EB-START - Enter");

            var dead = _thread == null || !_thread.IsAlive;
            if (dead) {
                _isRunning = true;
                _isStopping = false;

                _thread = new Thread(EngineThread) {Name = Name};
                _thread.Start(metrics);
            }
            
            if (Logger.IsTraceEnabled)
                Logger.Trace("EB-START - Exit : " + !dead);

            return !dead;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public bool Stop() {
            if (Logger.IsTraceEnabled)
                Logger.Trace("EB-STOP - Enter");

            var alive = (_thread != null && !_thread.IsAlive) || _isStopping;
            if (alive)
                _isStopping = true;

            if (Logger.IsTraceEnabled)
                Logger.Trace("EB-STOP - Exit : " + alive);

            return alive;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFiles"></param>
        /// <returns></returns>
        public IEnumerable<LogFile> GetProcessable(IEnumerable<LogFile> logFiles) {
            return logFiles.ToList().FindAll(TestIfProcessable);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metrics"></param>
        protected virtual void OnProgressChanged(EngineMetrics metrics) {
            OnProgressChanged(metrics, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metrics"></param>
        /// <param name="logFile"></param>
        protected virtual void OnProgressChanged(EngineMetrics metrics, LogFile logFile) {
            if (ProgressChanged == null) return;

            var percent = 0;

            if (metrics.ProcessedBytes > 0 && metrics.TotalBytes > 0) {
                percent = (int) (((float) metrics.ProcessedBytes/(float) metrics.TotalBytes)*100);
            } else if (metrics.ProcessedLogs > 0 && metrics.TotalLogs > 0) {
                percent = (int) (((float) metrics.ProcessedLogs/(float) metrics.TotalLogs)*100);
            }
            ProgressChanged.Invoke(this, new EngineProgressChangedEventArgs(percent, metrics, logFile));
            //Task.Factory.StartNew(() => ProgressChanged.Invoke(this, new EngineProgressChangedEventArgs(percent, metrics, logFile)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metrics"></param>
        protected virtual void OnCompleted(EngineMetrics metrics) {
            OnCompleted(null, false, metrics);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="cancelled"></param>
        /// <param name="metrics"></param>
        protected virtual void OnCompleted(Exception exception, bool cancelled, EngineMetrics metrics) {
            if (Completed != null) {
                Completed.Invoke(this, new EngineCompletedEventArgs(exception, cancelled, metrics));
            }
        }
        
        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="engineMetrics"></param>
        private void EngineThread(object engineMetrics) {
            var notified = false;

            if (Logger.IsTraceEnabled)
                Logger.Trace("EB-ET - Enter : " + Name);

            var state = new EngineState();
            var metrics = engineMetrics as EngineMetrics ?? new EngineMetrics();

            try {
                while (!_isStopping && _isRunning) {
                    var logFiles = GetProcessable(SessionInstance.LogFiles.ToList()).ToArray();
                    if (logFiles.Length == 0) {
                        if (Logger.IsTraceEnabled)
                            _logger.Trace("EB-ET-002", "No log files found to process");

                        break;
                    }

                    // Update metrics to reflect newly found log files
                    AddToMetrics(logFiles, ref metrics);

                    ProcessStart();
                    OnInfo(String.Format("Found {0} log files to process in the {1} engine.", logFiles.Length, Name));

                    foreach (var log in logFiles) {
                        state.LogFile = log;

                        if (_isStopping || !_isRunning)
                            break;

                        if (Logger.IsTraceEnabled)
                            _logger.Trace("EB-ET-003", "Processing: " + log);

                        var sw = Stopwatch.StartNew();
                        bool success;

                        try {
                            success = ProcessLog(log, ref metrics);
                        } catch (Exception ex) {
                            OnError("Failed to process log \"" + log.Filename + "\"", ex);
                            success = false;
                        }

                        sw.Stop();

                        metrics.TotalMilliseconds += sw.ElapsedMilliseconds;

                        SetLogProcessStatus(log, success ? EngineStatusEnum.Completed : EngineStatusEnum.Failed);
                        OnProgressChanged(metrics, log);

                        ProcessComplete(log, success, sw.ElapsedMilliseconds);
                    }
                }
            } catch (Exception ex) {
                OnCompleted(ex, _isStopping, metrics);

                // Set flag to indicate we've notified of completion
                notified = true;
            }

            _isStopping = false;
            _isRunning = false;

            if (!notified)
                OnCompleted(null, _isStopping, metrics);

            if (Logger.IsDebugEnabled)
                Logger.Debug(metrics.ToString());

            if (Logger.IsTraceEnabled)
                _logger.Trace("EB-ET - Exit : " + Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFiles"></param>
        /// <param name="metrics"></param>
        private void AddToMetrics(LogFile[] logFiles, ref EngineMetrics metrics) {
            var bytes = 0L;
            var len = logFiles.Length;

            for (var i = 0; i < len; i++)
                bytes += logFiles[i].Filesize;

            metrics.TotalBytes += bytes;
            metrics.TotalLogs += len;
        }

        #endregion

        #region Abstract methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <returns></returns>
        public abstract bool TestIfProcessable(LogFile logFile);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract void ProcessStart();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="engineMetrics"></param>
        /// <returns>True if the log file was processed successfully; otherwise, false.</returns>
        protected abstract bool ProcessLog(LogFile logFile, ref EngineMetrics engineMetrics);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="success"></param>
        /// <param name="timeElapsed"></param>
        protected abstract void ProcessComplete(LogFile logFile, bool success, long timeElapsed);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="status"></param>
        protected abstract void SetLogProcessStatus(LogFile logFile, EngineStatusEnum status);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <returns></returns>
        protected abstract EngineStatusEnum GetLogProcessStatus(LogFile logFile);

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

            GC.SuppressFinalize(this);
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
                    //if (? != null) {
                    //    ?.Dispose();
                    //}
                }

                // = null;
            } finally {
                IsDisposed = true;
            }
        }

        #endregion
    }
}
