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
    public sealed class FormatFieldCollection : CollectionBase<FormatField>
    {
        #region Initializers
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public FormatFieldCollection(UserSession session) : base(session) {

        }

        #endregion

        #region Indexers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override FormatField this[int index] {
            get { return _items[index]; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<FormatField> Find(string name) {
            var items = new List<FormatField>();

            lock (_locker) {
                for (var i = 0; i < _items.Count; i++) {
                    if (name.Equals(_items[i].Name)) {
                        items.Add(_items[i]);
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="contextFlags"></param>
        /// <returns></returns>
        public FormatField Find(string name, FieldContextFlags contextFlags) {
            lock (_locker) {
                for (var i = 0; i < _items.Count; i++) {
                    if (name.Equals(_items[i].Name)) {
                        return _items[i];
                    }
                }
            }

            return null;
        }

        #endregion
    }
}
