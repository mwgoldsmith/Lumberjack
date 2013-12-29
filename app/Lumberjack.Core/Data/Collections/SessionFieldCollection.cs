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
            for (var i = 0; i < _items.Count; i++) {
                if (_items[i].Name.Equals(name))
                    return _items[i];
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextFlags"></param>
        /// <returns></returns>
        public IEnumerable<SessionField> FindAll(FieldContextFlags contextFlags) {
            var fields = _items.Where(f => (f.ContextFlags & contextFlags) != 0);

            return fields;
        }

        #endregion
    }
}
