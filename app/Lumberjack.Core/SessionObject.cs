using System;
using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;
using Medidata.Lumberjack.Core.Logging;

namespace Medidata.Lumberjack.Core
{
    /// <summary>
    /// The MessageEmitterBase provides basic methods to send formatted text messages
    /// to subscribers of the OnMessage event handler. Any object which may provide
    /// either status updates or error messages should derive from this base class.
    /// </summary>
    public abstract class SessionObject
    {
        #region Constants

        // Maximum number of inner exceptions to include in formated message text
        private const int MaxInnerExceptions = 4;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        protected SessionObject() : this(null) {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        protected SessionObject(UserSession session) {
            SessionInstance = session ?? (this is UserSession ? this as UserSession : null);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public UserSession SessionInstance { get; protected set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Invokes the message event handler with the given debug message.
        /// </summary>
        /// <param name="message">The status message text.</param>
        [Conditional("DEBUG")]
        protected virtual void OnDebug(string message) {
            var e = new MessageEventArgs(MessageTypeEnum.Debug, message);

            SendToLog(e);
            SessionInstance.InvokeMessage(this, new MessageEventArgs(MessageTypeEnum.Debug, "[DEBUG] " + message));
        }

        /// <summary>
        /// Invokes the message event handler with the given status message.
        /// </summary>
        /// <param name="message">The status message text.</param>
        protected virtual void OnInfo(string message) {
            var e = new MessageEventArgs(MessageTypeEnum.Status, message);

            SendToLog(e);
            SessionInstance.InvokeMessage(this, e);
        }

        /// <summary>
        /// Invokes the message event handler with the given error message.
        /// </summary>
        /// <param name="message">The error message text.</param>
        protected virtual void OnError(string message) {
            OnError(message, null);
        }

        /// <summary>
        /// Invokes the message event handler with a formatted error message from
        /// the given exception.
        /// </summary>
        /// <param name="ex">The exception which occurred.</param>
        protected virtual void OnError(Exception ex) {
            OnError(null, ex);
        }

        /// <summary>
        /// Invokes the message event handler with a formatted error message from
        /// the given exception.
        /// </summary>
        /// <param name="message">
        ///     Alternate error message to display instead of the exception's message.
        /// </param>
        /// <param name="ex">The exception which occurred.</param>
        protected virtual void OnError(string message, Exception ex) {
            var e = new MessageEventArgs(MessageTypeEnum.Exception, message, ex);

            SendToLog(e);
            SessionInstance.InvokeMessage(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected static string FormatException(Exception ex) {
            return FormatException(String.Format("An exception has occured: \"{0}\"", ex.Message), ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected static string FormatException(string message, Exception ex) {
            var exception = ex;
            var msg = new StringBuilder();
            var count = 0;

            // Created formatted message from exception & inner exceptions
            while (exception != null) {
                count++;

                if (count == 1) {
                    msg.AppendFormat("{0}\r\n\r\nStacktrace:\r\n{1}", message, ex.StackTrace);
                } else if (count < MaxInnerExceptions) {
                    msg.AppendFormat("\r\n\r\nInner exception [{0}]: \"{1}\"\r\n\r\nStacktrace:\r\n{2}", count - 1, ex.Message, ex.StackTrace);
                } else {
                    msg.AppendFormat("\r\n\r\nInner exception [{0}]: ...", count - 1);
                    break;
                }

                exception = ex.InnerException;
            }

            return msg + "\r\n";
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private static void SendToLog(MessageEventArgs e) {
            var logger = Logger.GetInstance();

            try {
                if (logger != null && e.MessageType == MessageTypeEnum.Debug) {
                    logger.Debug(e.Message, e.Exception);
                } else if (logger != null && e.MessageType == MessageTypeEnum.Status) {
                    logger.Info(e.Message);
                } else if (e.MessageType == MessageTypeEnum.Exception) {
                    if (logger == null) {
                        EventLogger.Error(e.Message);
                    } else {
                        logger.Error(e.Message, e.Exception);
                    }
                } else if (e.MessageType == MessageTypeEnum.Error) {
                    if (logger == null) {
                        EventLogger.Error(e.Message);
                    } else {
                        logger.Error(e.Message);
                    }
                }
            } catch (Exception ex) {
                EventLogger.Error(FormatException(ex));

                throw;
            }
        }
        
        #endregion
    }
}