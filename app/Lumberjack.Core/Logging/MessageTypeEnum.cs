namespace Medidata.Lumberjack.Core.Logging
{
    /// <summary>
    /// Enumeration used to indicate the type of a message which is sent from
    /// a message-emitting object.
    /// </summary>
    public enum MessageTypeEnum
    {
        /// <summary>
        /// The message is a debugging message (conditional to DEBUG configuration)
        /// </summary>
        Debug,
        /// <summary>
        /// The message is a status update
        /// </summary>
        Status,
        /// <summary>
        /// The message is being sent after an error occurred
        /// </summary>
        Error,
        /// <summary>
        /// The message is being sent after an exception occurred
        /// </summary>
        Exception
    }
}
