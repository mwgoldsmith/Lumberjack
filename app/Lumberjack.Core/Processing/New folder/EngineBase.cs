using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Logging;

namespace Medidata.Lumberjack.Core.Processors
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class EngineBase : SessionObject, IDisposable
    {
        #region Private and protected fields

        protected static readonly object _locker = new object();

        private volatile bool _isRunning;
        private volatile bool _isPaused;
        private volatile bool _isStopping;

        protected Thread _thread;

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
            Name = GetType().Name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

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
        public bool IsPaused {
            get { return _isPaused; }
            protected set { _isPaused = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsStopping {
            get { return _isStopping; }
            protected set { _isStopping = value; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool Start() {
            if (_thread != null && _thread.IsAlive)
                return false;

            IsRunning = true;
            IsStopping = false;
            IsPaused = false;

            _thread = new Thread(EngineThread) { Name = Name };
            _thread.Start();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Stop() {
            if (!_thread.IsAlive || _isStopping)
                return false;

            _isStopping = true;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Pause() {
            if (!_thread.IsAlive || _isPaused)
                return false;

            _isPaused = true;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Resume() {
            if (!_thread.IsAlive || !_isPaused)
                return false;

            _isPaused = false;

            return true;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        private void EngineThread() {
            var state = new EngineState(SessionInstance);
            var logger = Logger.GetInstance();

            logger.Trace("ET-001");

            //var metrics = new EngineMetrics();

            try {
                while (!IsStopping && IsRunning) {
                    while (IsPaused && !IsStopping) {
                        Thread.Sleep(1000);
                    }
                    
                    var logFiles = GetLogFilesToProcess().ToArray();
                    if (logFiles.Length == 0)
                        break;

                    ProcessStart(state);
                    OnInfo(String.Format("Found {0} log files to process in the {1} engine.", logFiles.Length, Name));

                    foreach (var log in logFiles) {
                        state.LogFile = log;

                        Logger.GetInstance().Trace("ET-002: " + log);

                        var sw = Stopwatch.StartNew();
                        bool success;

                        try {
                            success = ProcessLog(state);
                        } catch (Exception ex) {
                            OnError("Failed to process log \"" + log.Filename + "\"", ex);
                            success = false;
                        }

                        sw.Stop();

                        //UpdateMetrics(ref metrics, sw.ElapsedMilliseconds, log);

                        if (success) {
                            SetLogProcessStatus(log, EngineStatusEnum.Completed);
                            OnDebug(String.Format("The {0} engine completed processing the log {1}.", Name, log.Filename));
                            SessionInstance.ProcessController.InvokeLogAction(this, EngineActionEnum.LogProcessed, log, null);// metrics.Clone());
                        } else {
                            SetLogProcessStatus(log, EngineStatusEnum.Failed);
                            OnDebug(String.Format("The {0} engine FAILED to process log {1}.", Name, log.Filename));
                            SessionInstance.ProcessController.InvokeLogAction(this, EngineActionEnum.LogUpdated, log, null);//metrics.Clone());
                        }

                        try {
                            if (!ProcessComplete(state, success))
                                break;
                        } catch (Exception ex) {
                            // Engine needs to be shut off at this point
                        }

                    }
                }
            } catch (Exception ex) {
                logger.Error("Engine failure", ex);
            }

            IsStopping = false;
            IsPaused = false;
            IsRunning = false;

            logger.Trace("ET-009");
        }

        
        #endregion


        #region Abstract methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<LogFile> GetLogFilesToProcess();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected abstract void ProcessStart(EngineState state);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected abstract bool ProcessLog(EngineState state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="success"></param>
        protected abstract bool ProcessComplete(EngineState state, bool success);

        protected abstract void SetLogProcessStatus(LogFile logFile, EngineStatusEnum status);

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
