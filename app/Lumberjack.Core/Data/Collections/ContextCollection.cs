using System.Diagnostics;

namespace Medidata.Lumberjack.Core.Data.Collections
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
        /// <param name="index"></param>
        /// <returns></returns>
        public override ContextFormat this[int index] {
            [DebuggerStepThrough]
            get { return _items[index]; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextType"></param>
        /// <returns></returns>
        public ContextFormat this[FormatContextEnum contextType] {
            [DebuggerStepThrough]
            get { return _items[GetContextEnumIndex(contextType)]; }
            [DebuggerStepThrough]
            set { _items[GetContextEnumIndex(contextType)] = value; }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextEnum"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
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
