using System.Diagnostics;
using Medidata.Lumberjack.Core.Data.Formats;

namespace Medidata.Lumberjack.Core.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ContextCollection : CollectionBase<ContextFormat>
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public ContextCollection(UserSession session)
            : base(session) {
            _items.AddRange(new ContextFormat[] {null, null, null});
        }

        #endregion

        #region Indexers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatContext"></param>
        /// <returns></returns>
        public ContextFormat this[FormatContextEnum formatContext] {
            [DebuggerStepThrough]
            get { return _items[GetContextEnumIndex(formatContext)]; }
            [DebuggerStepThrough]
            set { _items[GetContextEnumIndex(formatContext)] = value; }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextEnum"></param>
        /// <returns></returns>
        private static int GetContextEnumIndex(FormatContextEnum contextEnum) {
            switch (contextEnum) {
                case FormatContextEnum.Filename:
                    return 0;
                case FormatContextEnum.Entry:
                    return 1;
                case FormatContextEnum.Content:
                    return 2;
            }

            return -1;
        }

        #endregion
    }
}
