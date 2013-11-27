namespace Medidata.Lumberjack.Logging.Handlers.Sftp
{
    /// <summary>
    /// 
    /// </summary>
    public enum SecureErrorTypeEnum
    {
        None,
        SocketFailed,
        UnreadableKey,
        AuthenticationFailed,
        Disconnected,
        UnknownException
    }
}
