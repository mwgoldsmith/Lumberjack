using System;
using System.Collections.Generic;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Data.Fields;
using Medidata.Lumberjack.Core.Data.Fields.Values;

namespace Medidata.Lumberjack.Core.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class FieldValueCollection : CollectionBase<FieldValue>
    {
        /*
        // TODO: implement. Use a seperate bucket for each string length to boost performance
        // when searching for string field values
        private class StringBuckets
        {
            private readonly List<string>[] _buckets;
            private List<string> _extraBucket = new List<string>();

            public StringBuckets(int numBuckets) {
                _buckets = new List<string>[Math.Max(numBuckets, 20)];
                for (var i=0;i<numBuckets;i++)
                    _buckets[i] = new List<string>();
            }
        }
        */

        #region Event Handlers

        /// <summary>
        /// 
        /// </summary>
        public event ValueUpdatedHandler ValueUpdated;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public FieldValueCollection(UserSession session)
            : base(session) {
        }

        #endregion

        #region Indexers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public FieldValue this[int index] {
            get { return _items[index]; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldItem"></param>
        /// <param name="formatField"></param>
        /// <param name="value"></param>
        public void Add<T>(FieldItemBase fieldItem, FormatField formatField, T value) {
            Add(FieldValueFactory.CreateLogValue(fieldItem, formatField, value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldItem"></param>
        /// <param name="fieldComponent"></param>
        /// <returns></returns>
        public FieldValue Find<T>(FieldItemBase fieldItem, T fieldComponent)
            where T : FieldKeyedBase<T>, IFieldValueComponent {
            if (fieldItem == null)
                throw new ArgumentNullException("fieldItem");
            if (fieldComponent == null)
                throw new ArgumentNullException("fieldComponent");
            var len = _items.Count;

            for (var i = 0; i < len; i++) {
                var f = _items[i];

                if (f.Item.Equals(fieldItem) && f.ContainsComponent(fieldComponent))
                    return f;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public List<FieldValue> FindAll(Entry entry) {
            return FindAll(x => x.Item, entry.Id);
        }


        /// <summary>
        /// Finds all existing values for the specified FormatField.
        /// </summary>
        /// <param name="formatField"></param>
        /// <returns></returns>
        public List<FieldValue> FindAll(FormatField formatField) {
            return FindAll(x => x.FormatField, formatField.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <returns></returns>
        public List<FieldValue> FindAll(LogFile logFile) {
            return FindAll(x => x.Item, logFile.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionField"></param>
        /// <returns></returns>
        public List<FieldValue> FindAll(SessionField sessionField) {
            return FindAll(x => x.SessionField, sessionField.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldItem"></param>
        /// <param name="formatField"></param>
        /// <param name="value"></param>
        public void Update<T>(FieldItemBase fieldItem, FormatField formatField, T value) {
            var changed = false;

            lock (_locker) {
                var len = _items.Count;

                // Remove the field value lookup if it already exists
                for (var i = 0; i < len; i++) {
                    var f = _items[i];

                    if (f.ContainsComponent(fieldItem) && f.ContainsComponent(formatField)) {
                        _items.Remove(_items[i]);
                        changed = true;
                        break;
                    }
                }
            }

            // Add the field value
            Add(fieldItem, formatField, value);

            OnValueUpdated(fieldItem, formatField, value, changed);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<FieldValue> FindAll(Func<FieldValue, IFieldValueComponent> selector, int id) {
            var result = new List<FieldValue>();

            lock (_locker) {
                var len = _items.Count;
                for (var i = 0; i < len; i++) {
                    var item = _items[i];

                    var valSelector = selector(item);
                    if (valSelector != null && valSelector.Id == id)
                        result.Add(item);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldItem"></param>
        /// <param name="formatField"></param>
        /// <param name="value"></param>
        /// <param name="changed"></param>
        private void OnValueUpdated(FieldItemBase fieldItem, FormatField formatField, object value, bool changed) {
            if (ValueUpdated != null) {
                ValueUpdated(this, new ValueUpdatedEventArgs(fieldItem, formatField, value, changed));
            }
        }

        #endregion

    }
}