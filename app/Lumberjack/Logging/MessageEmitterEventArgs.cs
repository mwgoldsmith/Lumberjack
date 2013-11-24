using System;

namespace Medidata.Lumberjack.Logging
{
    #region Delegates

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public delegate void MessageEmitterHandler(object source, MessageEmitterEventArgs e);

    #endregion

    #region Enums

    /// <summary>
    /// 
    /// </summary>
    public enum MessageTypeEnum
    {
        Status,
        Error,
        Exception
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public class MessageEmitterEventArgs : EventArgs
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="message"></param>
        public MessageEmitterEventArgs(MessageTypeEnum messageType, object message)
        {
            MessageType = messageType;
            Message = message;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public MessageTypeEnum MessageType { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public object Message { get; protected set; }

        #endregion
    }

}
