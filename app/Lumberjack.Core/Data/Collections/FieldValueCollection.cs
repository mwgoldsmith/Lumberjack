using System;
using System.Collections;
using System.Collections.Generic;

namespace Medidata.Lumberjack.Core.Data.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class FieldValueCollection : CollectionBase<FieldValueLookup>
    {
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

        #region Private fields

        private readonly Dictionary<FieldDataTypeEnum, IList> _values;

        #endregion

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
        public FieldValueCollection(UserSession session) : base(session) {
            _values = new Dictionary<FieldDataTypeEnum, IList>
                {
                    {FieldDataTypeEnum.DateTime, new List<DateTime>()},
                    {FieldDataTypeEnum.Integer, new List<Int32>()},
                    {FieldDataTypeEnum.String, new List<string>()}
                };
        }

        #endregion

        #region Indexers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override FieldValueLookup this[int index] {
            get { return _items[index]; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container">The object the value is related to. (ex: LogFile, Entry)</param>
        /// <param name="formatField"></param>
        /// <returns></returns>
        public object Find(IFieldValueContainer container, FormatField formatField) {
            if (container == null)
                throw new ArgumentNullException("container");
            if (formatField == null)
                throw new ArgumentNullException("formatField");
            
            var containerId = container.Id;
            var formatFieldId = formatField.Id;

            for (var i = 0; i < _items.Count; i++) {
                var item = _items[i];
                if (item.ContainerId == containerId && item.FormatField.Id == formatFieldId) {
                    return _values[formatField.DataType][item.Index];
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="sessionField"></param>
        /// <returns></returns>
        public object Find(IFieldValueContainer container, SessionField sessionField) {
            if (container == null)
                throw new ArgumentNullException("container");
            if (sessionField == null)
                throw new ArgumentNullException("sessionField");

            var containerId = container.Id;
            var sessionFieldId = sessionField.Id;

            for (var i = 0; i < _items.Count; i++) {
                var item = _items[i];
                if (item.ContainerId == containerId && item.FormatField.SessionField.Id == sessionFieldId) {
                    return _values[item.FormatField.DataType][item.Index];
                }
            }

            return null;
        }

        /// <summary>
        /// Finds all existing values for all FormatFields which are linked to
        /// a specified SessionField.
        /// </summary>
        /// <param name="sessionField"></param>
        /// <returns></returns>
        public IEnumerable<object> FindAll(SessionField sessionField) {
            var result = new List<object>();

            lock (_locker) {
                for (var i = 0; i < _items.Count; i++) {
                    var item = _items[i];
                    if (ReferenceEquals(item.FormatField.SessionField, sessionField)) {
                        result.Add(_values[item.FormatField.DataType][item.Index]);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Finds all existing values for the specified FormatField.
        /// </summary>
        /// <param name="formatField"></param>
        /// <returns></returns>
        public IEnumerable<object> FindAll(FormatField formatField) {
            var result = new List<object>();

            lock (_locker) {
                for (var i = 0; i < _items.Count; i++) {
                    var item = _items[i];
                    if (ReferenceEquals(item.FormatField, formatField)) {
                        result.Add(_values[item.FormatField.DataType][item.Index]);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="formatField"></param>
        /// <param name="value"></param>
        public void Add<T>(IFieldValueContainer container, FormatField formatField, T value)  {
            if (typeof(T) == typeof(string) && formatField.DataType != FieldDataTypeEnum.String) {
                if (formatField.DataType == FieldDataTypeEnum.DateTime) {
                    DateTime dateValue = new DateTime();

                    if (!formatField.TryUnformatValue(value.ToString(), ref dateValue))
                        return;

                    Add(container, formatField, dateValue);
                } else if (formatField.DataType == FieldDataTypeEnum.Integer) {
                    Int32 intValue = 0;

                    if (!formatField.TryUnformatValue(value.ToString(), ref intValue))
                        return;

                    Add(container, formatField, intValue);
                }
            } else {
                lock (_locker) {
                    var values = _values[formatField.DataType];

                    var index = ((List<T>) values).IndexOf(value);

                    if (index == -1) {
                        index = values.Count;
                        values.Add(value);
                    }

                    Add(new FieldValueLookup(container.Id, formatField, index));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="formatField"></param>
        /// <param name="value"></param>
        public void Update<T>(IFieldValueContainer container, FormatField formatField, T value) {
            var containerId = container.Id;
            var formatFieldId = formatField.Id;
            var changed = false;

            lock (_locker) {
                // Remove the field value lookup if it already exists
                for (var i = 0; i < _items.Count; i++) {
                    var item = _items[i];
                    if (item.ContainerId != containerId || item.FormatField.Id != formatFieldId) 
                        continue;

                    _items.Remove(_items[i]);
                    changed = true;
                    break;
                }

                // Add the field value
                Add(container, formatField, value);
            }

            OnValueUpdated(container, formatField, value, changed);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="formatField"></param>
        /// <param name="value"></param>
        /// <param name="changed"></param>
        private void OnValueUpdated(IFieldValueContainer container, FormatField formatField, object value, bool changed) {
            if (ValueUpdated != null) {
                ValueUpdated(this, new ValueUpdatedEventArgs(container, formatField, value, changed));
            }
        }

        #endregion

    }
}