namespace Medidata.Lumberjack.Core
{
    /// <summary>
    /// 
    /// </summary>
    public static class SessionFactory
    {
        #region Public static methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static UserSession GetSession() {
            return GetSession(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static UserSession GetSession(SessionSettings settings) {
            return new UserSession(settings);
        }

        #endregion
    }
}
