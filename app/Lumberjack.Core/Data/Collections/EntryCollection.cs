using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;

namespace Medidata.Lumberjack.Core.Data.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class EntryCollection : CollectionBase<Entry>
    {
        #region Nested structs

        /// <summary>
        /// 
        /// </summary>
        public struct EntryValuePair
        {
            /// <summary>
            /// 
            /// </summary>
            public Entry Entry { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public object Value { get; set; }
        }

        #endregion

        #region Private fields

        private static long _id;

        private static CaseInsensitiveComparer _objectCompare = new CaseInsensitiveComparer();

        #endregion

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
        /// <param name="sessionField"></param>
        /// <param name="order"></param>
        public void Sort(SessionField sessionField, SortOrder order) {
            EntryValuePair[] values;
            int len;
           
            /* Lock for the entire duration.
             * If not, since the inner-list isn't being sorted in place, any items
             * added or removed while sorting would be lost.
             * 
             * Performance might be a problem here... if so, find another way to sort.
             */
            lock (_locker) {
                len = _items.Count;
                values = new EntryValuePair[len];

                // Create array of Entry to Field Value pairs to sort
                for (var i = 0; i < len; i++) {
                    var entry = _items[i];
                    var value = SessionInstance.FieldValues.Find(entry, sessionField);
                    values[i] = new EntryValuePair {Entry = entry, Value = value};
                }

                // Sort the pairs based on the field's data type
                Array.Sort(values, GetValueComparer(sessionField.DataType, order));

                // Create a new list with the entries in sorted order
                var items = new List<Entry>(len);
                for (var i = 0; i < len; i++)
                    items.Add(values[i].Entry);

                // Replace the internal list with the new, sorted one
                _items = items;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private static Comparison<EntryValuePair> GetValueComparer(FieldDataTypeEnum dataType, SortOrder order) {
            var direction = order == SortOrder.Ascending ? 1 : -1;

            if (dataType == FieldDataTypeEnum.DateTime) {
                return  (x, y) => _objectCompare.Compare((DateTime)x.Value, (DateTime)y.Value) * direction;
            } 
            
            if (dataType == FieldDataTypeEnum.Integer) {
                return (x, y) => _objectCompare.Compare((Int32)x.Value, (Int32)y.Value) * direction;
            }

            return (x, y) => _objectCompare.Compare(x.Value, y.Value)*direction;
        }

        #endregion
    }
}
