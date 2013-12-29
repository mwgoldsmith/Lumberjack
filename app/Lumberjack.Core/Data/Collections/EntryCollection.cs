using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medidata.Lumberjack.Core.Data.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class EntryCollection : CollectionBase<Entry>
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
        public override Entry this[int index] {
            get { return _items[index]; }
        }

        #endregion
    }
}
