using Medidata.Lumberjack.Core.Data.Formats;

namespace Medidata.Lumberjack.Core.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SessionFormatCollection : CollectionBase<SessionFormat>
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public SessionFormatCollection(UserSession session)
            : base(session) {
            
        }

        #endregion

        #region Indexers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SessionFormat this[int index] {
            get { return _items[index]; }
        }

        #endregion
    }
}
