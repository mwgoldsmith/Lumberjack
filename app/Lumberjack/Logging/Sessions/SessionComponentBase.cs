namespace Medidata.Lumberjack.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SessionComponentBase : MessageEmitterBase
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        protected SessionComponentBase(Session session)
        {
            Session = session;
        }

        #endregion

        #region Protected properties

        /// <summary>
        /// 
        /// </summary>
        protected Session Session { get; set; }

        #endregion
    }
}
