using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Medidata.Lumberjack.Core.Data.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class FormatFieldCollection : CollectionBase<FormatField>
    {
        private static long _id;

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
            [DebuggerStepThrough]
            get { return _items[index]; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static int GetNextId() {
            return (int)Interlocked.Increment(ref _id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<FormatField> Find(string name) {
            var items = new List<FormatField>();

            lock (_locker) {
                var len = _items.Count;

                for (var i = 0; i < len; i++) {
                    var item = _items[i];

                    if (name.Equals(item.Name))
                        items.Add(item);
                }
            }

            return items;
        }

        /// <summary>
        /// Finds all FormatField items in the collection which are linked to a
        /// specified SessionField.
        /// </summary>
        /// <param name="sessionField">The SessionField object.</param>
        /// <returns></returns>
        public List<FormatField> Find(SessionField sessionField) {
            var items = new List<FormatField>();
            var id = sessionField.Id;

            lock (_locker) {
                for (var i = 0; i < _items.Count; i++) {
                    var item = _items[i];
                    if (item.SessionField != null && item.SessionField.Id == id)
                        items.Add(item);
                }
            }

            return items;
        }
        
        /// <summary>
        /// Finds the first FormatField item in the collection which is linked to a
        /// specified SessionField. 
        /// </summary>
        /// <param name="sessionField">The SessionField object.</param>
        /// <returns></returns>
        public FormatField FindFirst(SessionField sessionField) {
            var id = sessionField.Id;

            lock (_locker) {
                var len = _items.Count;

                for (var i = 0; i < len; i++) {
                    var item = _items[i];
                    if (item.SessionField != null && item.SessionField.Id == id)
                        return item;
                }
            }

            return null;
        }

        #endregion
    }
}
