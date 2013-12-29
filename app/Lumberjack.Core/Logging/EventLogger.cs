using System;
using System.Diagnostics;
using System.Security.Permissions;

namespace Medidata.Lumberjack.Core.Logging
{
    internal sealed class EventLogger
    {
        #region Constants

        private const string Source = "Lumberjack";
        private const string Log = "Application";

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlAppDomain)]
        static EventLogger() {
            if (!EventLog.SourceExists(Source)) {
                EventLog.CreateEventSource(Source, Log);
            }

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void Write(string message) {
            EventLog.WriteEntry(Source, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public static void Write(string message, EventLogEntryType type) {
            Debug.Write(message, type.ToString());
            EventLog.WriteEntry(Source, message, type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message) {
            Write(message, EventLogEntryType.Error);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message) {
            Write(message, EventLogEntryType.Information);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void Warn(string message) {
            Write(message, EventLogEntryType.Warning);
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            var ex = (Exception)e.ExceptionObject;
            var message = "An unhandled exception occurred"
                          + (e.IsTerminating ? " and has been termined" : ".")
                          + "\r\n\r\nMessage: " + ex.Message + "\r\n" +
                          "Source: " + ex.Source + "\r\n" +
                          "Stacktrace:" + ex.StackTrace;

            Error(message);
        }

        #endregion
    }
}
