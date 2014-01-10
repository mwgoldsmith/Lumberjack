using System;
using System.Collections;
using System.Collections.Generic;
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
            get { return _items[index]; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static long GetNextId() {
            return Interlocked.Increment(ref _id);
        }

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
        /// Finds all FormatField items in the collection which are linked to a
        /// specified SessionField.
        /// </summary>
        /// <param name="sessionField">The SessionField object.</param>
        /// <returns></returns>
        public IEnumerable<FormatField> Find(SessionField sessionField) {
            var items = new List<FormatField>();

            lock (_locker) {
                for (var i = 0; i < _items.Count; i++) {
                    if (ReferenceEquals(sessionField, _items[i].SessionField)) {
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

        /// <summary>
        /// Finds all FormatField items in the collection which are linked to a
        /// specified SessionField. 
        /// </summary>
        /// <param name="sessionField">The SessionField object.</param>
        /// <returns></returns>
        public FormatField FindFirst(SessionField sessionField) {
            lock (_locker) {
                for (var i = _items.Count-1; i >= 0 ; i--) {
                    if (sessionField.Id == _items[i].SessionField.Id) {
                        return _items[i];
                    }
                }
            }

            return null;
        }

        #endregion
    }
}
