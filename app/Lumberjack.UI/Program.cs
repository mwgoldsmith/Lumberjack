using System;
using System.Threading;
using System.Windows.Forms;
using Medidata.Lumberjack.Core;
using Medidata.Lumberjack.Core.Logging;

namespace Medidata.Lumberjack.UI
{
    internal static class Program
    {
        #region Global properties

        /// <summary>
        /// The current user session
        /// </summary>
        public static UserSession UserSession { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public static Logger Logger { get; private set; }

        #endregion

        #region Main application

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main() {
            Logger = InitLogging();
            Logger.Trace("STARTUP");

            // Add handlers to hook unhandled exceptions and exiting the app
            Application.ApplicationExit += Application_ApplicationExit;
            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Instantiate application-scope user session 
            var configFiles = SettingsManager.LoadConfigFilenames();
            var settings = new SessionSettings
            {
                FieldConfigFile = configFiles.Fields,
                FormatConfigFile = configFiles.Formats,
                NodeConfigFile = configFiles.Nodes
            };

            UserSession = SessionFactory.GetSession(settings);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new MainForm());
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e) {
            Logger.Error("An uncaught thread exception occurred:", e.Exception);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_ApplicationExit(object sender, EventArgs e) {
            Logger.Trace("SHUTDOWN");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e) {
            Logger.Error("An unhandled exception occurred: { IsTerminating = " + e.IsTerminating + " }", e.ExceptionObject as Exception);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static Logger InitLogging() {
            var logger = Logger.GetInstance();

            var levels = SettingsManager.LoadLoggingLevels();

            Logger.IsFineEnabled = levels.FineEnabled;
            Logger.IsTraceEnabled = true;
          //  levels.TraceEnabled;
            Logger.IsDebugEnabled = levels.DebugEnabled;
            Logger.IsInfoEnabled = levels.InfoEnabled;
            Logger.IsWarnEnabled = levels.WarnEnabled;
            Logger.IsErrorEnabled = levels.WarnEnabled;

            return logger;
        }

        #endregion
    }
}