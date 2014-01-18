using System;

namespace Medidata.Lumberjack.Core.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public class CollectionItemEventArgs<T> : EventArgs
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public CollectionItemEventArgs(T[] items) {
            Items = items;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public T[] Items { get; private set; }

        #endregion
    }
}
