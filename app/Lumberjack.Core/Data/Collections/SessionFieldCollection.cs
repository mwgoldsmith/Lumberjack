using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Medidata.Lumberjack.Core.Data.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SessionFieldCollection : CollectionBase<SessionField>
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public SessionFieldCollection(UserSession session)
            : base(session) {
            
        }

        #endregion

        #region Indexers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override SessionField this[int index] {
            [DebuggerStepThrough]
            get { return _items[index]; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SessionField Find(string name) {
            var len = _items.Count;

            for (var i = 0; i < len; i++) {
                var item = _items[i];

                if (name.Equals(item.Name))
                    return item;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextFlags"></param>
        /// <returns></returns>
        public List<SessionField> FindAll(FieldContextFlags contextFlags) {
            var result = new List<SessionField>();
            var len = _items.Count;

            for (var i = 0; i < len; i++) {
                var item = _items[i];

                if ((item.ContextFlags & contextFlags) != 0)
                    result.Add(item);
            }

            return result;
        }

        #endregion
    }
}
