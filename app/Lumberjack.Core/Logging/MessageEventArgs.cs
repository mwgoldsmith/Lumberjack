using System;

namespace Medidata.Lumberjack.Core.Logging
{
    #region Delegates

    /// <summary>
    /// Delegate used for the event handler in classes derived from MessageEmitterBase.
    /// </summary>
    /// <param name="source">Object which is sending the message.</param>
    /// <param name="e">Event arguments for the event handler.</param>
    public delegate void MessageEventHandler(object source, MessageEventArgs e);

    #endregion

    #region Event arguments

    /// <summary>
    /// Event arguments utilized by the MessageHandler delegate.
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public MessageEventArgs(MessageTypeEnum messageType, string message, Exception exception)
            : this(messageType, DateTime.UtcNow, message) {
            Exception = exception;
        }

        /// <summary>
        /// Creates a new MessageEventArgs instance.
        /// </summary>
        /// <param name="messageType">Enum value indicating the type of message.</param>
        /// <param name="timestamp">Timestamp of the message.</param>
        /// <param name="message">The message text.</param>
        public MessageEventArgs(MessageTypeEnum messageType, DateTime timestamp, string message) {
            MessageType = messageType;
            Message = message;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Creates a new MessageEventArgs instance using the current time (UTC)
        /// as the timestamp.
        /// </summary>
        /// <param name="messageType">Enum value indicating the type of message.</param>
        /// <param name="message">The message text.</param>
        public MessageEventArgs(MessageTypeEnum messageType, string message)
            : this(messageType, DateTime.UtcNow, message) {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Enum value indicating the type of message.
        /// </summary>
        public MessageTypeEnum MessageType { get; protected set; }

        /// <summary>
        /// Timestamp of the message.
        /// </summary>
        public DateTime Timestamp { get; protected set; }

        /// <summary>
        /// The message text.
        /// </summary>
        public string Message { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public Exception Exception { get; protected set; }

        #endregion
    }

    #endregion
}
