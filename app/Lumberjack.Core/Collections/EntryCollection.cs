using System.Diagnostics;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Data.Fields.Values;

namespace Medidata.Lumberjack.Core.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class EntryCollection : CollectionBase<Entry>, IValueItemCollection<Entry>
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public EntryCollection(UserSession session)
            : base(session) {
            
        }

        #endregion

        #region Indexers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Entry this[int index] {
            [DebuggerStepThrough]
            get { return _items[index]; }
        }

        #endregion
    }
}
