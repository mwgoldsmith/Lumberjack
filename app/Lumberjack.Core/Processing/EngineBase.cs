using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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

        #region Private and protected fields

        protected static readonly object _locker = new object();

        private volatile bool _isRunning;
        private volatile bool _isStopping;

        private Thread _thread;
        protected Logger _logger;

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
        public EngineProgressChangedHandler ProgressChanged;

        /// <summary>
        /// 
        /// </summary>
        public EngineCompletedHandler Completed;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }

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
            if (_thread != null && _thread.IsAlive)
                return false;

            _isRunning = true;
            _isStopping = false;

            _thread = new Thread(EngineThread) { Name = Name };
            _thread.Start(metrics);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        /// <param name="container"></param>
        /// <param name="contextFormat"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        protected bool ParseFormatFields(LogFile log, IFieldValueContainer container, ContextFormat contextFormat, Match match) {
            var fields = contextFormat.Fields;

            if (Logger.IsTraceEnabled) {
                var containerStr = "";

                if (container is LogFile) {
                    containerStr = (container as LogFile).ToString();
                } else if (container is Entry) {
                    containerStr = (container as Entry).ToString();
                }

                var msg = String.Format("Entering. Args: {{ logFile = {0}, container = {1}, contextFormat = {2}, match = {3} (chars) }}", log, containerStr, contextFormat, match.Length);

                _logger.Trace("EB-PFF-001", msg);
            }

            // Iterate over each field the format can contain within the filename
            foreach (var field in fields) {
                string value = null;

                var groups = field.Groups;
                if (groups == null) {
                    // If there are no groups defined, the field is permitted as a FormatField but
                    // cannot be derived by parsing data in this (or possibly any) context
                    continue;
                }

                // Take the first capture which was successful
                for (var i = 0; i < groups.Length; i++) {
                    if (!match.Groups[groups[i]].Success)
                        continue;

                    value = match.Groups[groups[i]].ToString();
                    break;
                }

                if (field.Name.Equals(ContentField)) {
                    if (Logger.IsTraceEnabled)
                        _logger.Trace("EB-PFF-002", "Processing content format context");

                    // Parse the log entry content to retreive additional metadata
                    var contentContext = log.SessionFormat.Contexts[FormatContextEnum.Content];
                    var success = ParseAllFieldMatches(log, container, contentContext, value);

                    if (!success) {
                        OnDebug(String.Format("Failed to process CONTENT for log file {0}.", log.Filename));
                        return false;
                    }
                } else {
                    // Check if field was not found but has default value
                    if (value == null && field.Default != null) {
                        value = field.Default;
                    }

                    if (value != null) {
                        if (Logger.IsTraceEnabled) {
                            if (field.Name == "MESSAGE")
                                _logger.Trace("EB-PFF-003", "Field = '" + field.Name + "', Value = " + value.Length + " characters (len)");
                            else
                                _logger.Trace("EB-PFF-003", "Field = '" + field.Name + "', Value = '" + value + "'");
                        }

                        if (field.Name.Equals("LEVEL")) {
                            switch (value.ToUpper()) {
                                case "TRACE":
                                    log.EntryStats.Trace++;
                                    break;
                                case "DEBUG":
                                    log.EntryStats.Debug++;
                                    break;
                                case "INFO":
                                    log.EntryStats.Info++;
                                    break;
                                case "WARN":
                                    log.EntryStats.Warn++;
                                    break;
                                case "ERROR":
                                    log.EntryStats.Error++;
                                    break;
                                case "FATAL":
                                    log.EntryStats.Fatal++;
                                    break;
                            }    
                        }

                        
                        if (field.Filterable && !SetRawFieldValue(container, field, value)) {
                            OnError("Failed to set field '" + field.Name + "' to value '" + value + "'");
                            return false;
                        }
                    }
                }

                // Check if required field could not be found and no default value
                if (!field.Required || value != null)
                    continue;

                OnInfo(String.Format("Required field \"{0}\" not found for log file.", field.Name));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="container"></param>
        /// <param name="contextFormat"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        protected bool ParseAllFieldMatches(LogFile logFile, IFieldValueContainer container, ContextFormat contextFormat, string text) {
            var matches = contextFormat.Regex.Matches(text);

            foreach (Match match in matches) {
                if (match.Success && !ParseFormatFields(logFile, container, contextFormat, match)) {
                    return false;
                }
            }

            return true;
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
                _logger.Trace("EB-ET-001", "Entering " + Name);

            var state = new EngineState();
            var metrics = engineMetrics as EngineMetrics ?? new EngineMetrics();

            try {
                while (!_isStopping && _isRunning) {
                    var logFiles = GetLogFilesToProcess().ToArray();
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

                        ProcessComplete(log, success);
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

            //*/OnInfo(String.Format("Engine {0} has completed processing.", Name));
            //*/SessionInstance.ProcessController.InvokeLogAction(this, EngineActionEnum.ProcessTerminated, null, metrics);

            _logger.Trace(metrics.ToString());

            if (Logger.IsTraceEnabled)
                _logger.Trace("EB-ET-009", "Exiting " + Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="formatField"></param>
        /// <param name="value"></param>
        private bool SetRawFieldValue(IFieldValueContainer container, FormatField formatField, string value) {
            if (formatField.DataType == FieldDataTypeEnum.DateTime) {
                var dateValue = new DateTime();

                if (!formatField.TryUnformatValue(value, ref dateValue))
                    return false;

                SessionInstance.FieldValues.Add(container, formatField, dateValue);
            } else if (formatField.DataType == FieldDataTypeEnum.Integer) {
                var intValue = 0;

                if (!formatField.TryUnformatValue(value, ref intValue))
                    return false;

                SessionInstance.FieldValues.Add(container, formatField, intValue);
            } else if (formatField.DataType == FieldDataTypeEnum.String) {
                SessionInstance.FieldValues.Add(container, formatField, value);
            }

            return true;
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
        /// <returns></returns>
        protected abstract IEnumerable<LogFile> GetLogFilesToProcess();
        
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
        protected abstract void ProcessComplete(LogFile logFile, bool success);

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
