using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace Medidata.Lumberjack.Core.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Logger
    {
        #region Constants

        private const string DefaultLogFilename = "lumberjack.log";
        private const string ConfigFilename = "log4net.config";

        #endregion

        #region Private fields

        private readonly object _locker = new object();

        private readonly ILogger _logger;

        private static readonly Type _type;
        private static bool _isInitialized;
        private static Logger _instance;

        private static bool _isTraceEnabled;
        private static bool _isDebugEnabled;
        private static bool _isInfoEnabled = true;
        private static bool _isWarnEnabled = true;
        private static bool _isErrorEnabled = true;
        private static bool _isFatalEnabled = true;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        static Logger() {
            _type = MethodBase.GetCurrentMethod().DeclaringType;
            _isInitialized = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public Logger()
            : this(false) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="makeDefault"></param>
        public Logger(bool makeDefault)
            : this(DefaultLogFilename, makeDefault) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="makeDefault"></param>
        private Logger(string filename, bool makeDefault) {
            if (!_isInitialized) {
                Initialize();
            }

            _logger = LogManager.GetLogger(_type).Logger;
            if (filename != null) {
                XmlConfigurator.Configure();

                var file = Path.GetFileName(filename) ?? DefaultLogFilename;
                var h = (Hierarchy) LogManager.GetRepository();

                foreach (var a in h.Root.Appenders) {
                    if (!(a is FileAppender))
                        continue;

                    var fa = (FileAppender) a;
                    fa.File = Path.Combine(FileUtil.ApplicationPath, file);
                    fa.ActivateOptions();

                    break;
                }
            }

            _logger.Log(_type, Level.Trace, "ML-001", null);

            if (makeDefault) {
                _instance = this;
            }
        }

        #endregion

        #region Static properties

        /// <summary>
        /// 
        /// </summary>
        public static Logger DefaultLogger {
            get { return _instance ?? (_instance = new Logger()); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool IsFineEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static bool IsTraceEnabled {
            get { return _isTraceEnabled || IsFineEnabled; }
            set { _isTraceEnabled = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool IsDebugEnabled {
            get { return _isDebugEnabled || IsFineEnabled; }
            set { _isDebugEnabled = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool IsInfoEnabled {
            get { return _isInfoEnabled || IsFineEnabled; }
            set { _isInfoEnabled = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool IsWarnEnabled {
            get { return _isWarnEnabled || IsFineEnabled; }
            set { _isWarnEnabled = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool IsErrorEnabled {
            get { return _isErrorEnabled || IsFineEnabled; }
            set { _isErrorEnabled = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool IsFatalEnabled {
            get { return _isFatalEnabled || IsFineEnabled; }
            set { _isFatalEnabled = value; }
        }

        #endregion

        #region Static methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Logger GetInstance() {
            return _instance ?? (_instance = new Logger());
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Initialize() {
            Initialize(ConfigFilename);
        }

        /// <summary>
        /// Initializes the logger to use a specific config file.
        /// </summary>
        /// <param name="configFile">The path of the config file.</param>
        public static void Initialize(string configFile) {
            if (_isInitialized) {
                throw new NotSupportedException("Logging has already been initialized.");
            }

            if (!String.IsNullOrEmpty(configFile)) {
                XmlConfigurator.ConfigureAndWatch(new FileInfo(configFile));
            } else {
                XmlConfigurator.Configure();
            }

            _isInitialized = true;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Trace(string message) {
            if (!IsTraceEnabled)
                return;

            InternalTrace(GetTraceDetails(2), message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Trace(string message, Exception ex) {
            if (!IsTraceEnabled)
                return;

            InternalTrace(GetTraceDetails(2), message, ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public void Trace(string code, string message) {
            if (!IsTraceEnabled)
                return;

            InternalTrace(code, message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Trace(string code, string message, Exception ex) {
            if (!IsTraceEnabled)
                return;

            InternalTrace(code, message, ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Debug(string message) {
            if (IsDebugEnabled)
                Debug(message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Debug(string message, Exception ex) {
            if (IsDebugEnabled)
                _logger.Log(_type, Level.Debug, message + (ex != null ? "\r\n" : ""), ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Warn(string message) {
            if (IsWarnEnabled)
                Warn(message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Warn(string message, Exception ex) {
            if (IsWarnEnabled)
                _logger.Log(_type, Level.Warn, message + (ex != null ? "\r\n" : ""), ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message) {
            if (IsInfoEnabled)
                _logger.Log(_type, Level.Info, message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message) {
            if (IsErrorEnabled)
                Error(message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Error(string message, Exception ex) {
            if (IsErrorEnabled)
                _logger.Log(_type, Level.Error, message + (ex != null ? "\r\n" : ""), ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Fatal(string message) {
            if (IsFatalEnabled)
                Fatal(message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Fatal(string message, Exception ex) {
            if (IsFatalEnabled)
                _logger.Log(_type, Level.Fatal, message + (ex != null ? "\r\n" : ""), ex);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        private void InternalTrace(string caller, string message, Exception ex) {
            _logger.Log(_type, Level.Trace, (caller != null ? caller + " - " : "") + message + (ex != null ? "\r\n" : ""), ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetTraceDetails(int depth) {
            var frame = new StackFrame(depth, true);
            var method = frame.GetMethod();

            if (method.DeclaringType == null)
                return "";

            return method.DeclaringType.UnderlyingSystemType.FullName
                   + "." + method.Name + "() : "
                   + frame.GetFileLineNumber();
        }

        #endregion
    }
}
    
