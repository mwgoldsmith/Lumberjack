using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using log4net.Appender;

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

        private readonly List<DateTime> _dateValues;
        private readonly List<Int32> _intValues;
        private readonly List<string> _stringValues;

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
        public FieldValueCollection(UserSession session)
            : base(session) {
            _dateValues = new List<DateTime>();
            _intValues = new List<Int32>();
            _stringValues = new List<string>();
            /*_values = new Dictionary<FieldDataTypeEnum, IList>
                {
                    {FieldDataTypeEnum.DateTime, new List<DateTime>()},
                    {FieldDataTypeEnum.Integer, new List<Int32>()},
                    {FieldDataTypeEnum.String, new List<string>()}
                };*/
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
            
            return Find(container.Id, formatField.DataType, formatField.Id, false);
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

            return Find(container.Id, sessionField.DataType, sessionField.Id, true);
        }

        /// <summary>
        /// Finds all existing values for all FormatFields which are linked to
        /// a specified SessionField.
        /// </summary>
        /// <param name="sessionField"></param>
        /// <returns></returns>
        public List<object> FindAll(SessionField sessionField) {
            var id = sessionField.Id;
            var result = new List<object>();

            lock (_locker) {
                var len = _items.Count;
                for (var i = 0; i < len; i++) {
                    var item = _items[i];
                    var field = item.FormatField.SessionField;

                    if (field != null && field.Id == id)
                        result.Add(GetValue(item.FormatField.DataType, item.Index));
                }
            }

            return result;
        }

        /// <summary>
        /// Finds all existing values for the specified FormatField.
        /// </summary>
        /// <param name="formatField"></param>
        /// <returns></returns>
        public List<object> FindAll(FormatField formatField) {
            var id = formatField.Id;
            var result = new List<object>();

            lock (_locker) {
                var len = _items.Count;

                for (var i = 0; i < len; i++) {
                    var item = _items[i];
                    var field = item.FormatField;

                    if (field != null && field.Id == id)
                        result.Add(GetValue(item.FormatField.DataType, item.Index));
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
        public void Add<T>(IFieldValueContainer container, FormatField formatField, T value) {
            var dataType = formatField.DataType;

            if (typeof (T) == typeof (string) && dataType != FieldDataTypeEnum.String) {
                if (dataType == FieldDataTypeEnum.DateTime) {
                    DateTime dateValue = new DateTime();

                    if (formatField.TryUnformatValue(value.ToString(), ref dateValue))
                        Add(container, formatField, dateValue);
                } else if (dataType == FieldDataTypeEnum.Integer) {
                    Int32 intValue = 0;

                    if (formatField.TryUnformatValue(value.ToString(), ref intValue))
                        Add(container, formatField, intValue);
                }

                return;
            }

            IList values;
            int index;

            switch (dataType) {
                case FieldDataTypeEnum.DateTime:
                    values = _dateValues;
                    break;
                case FieldDataTypeEnum.Integer:
                    values = _intValues;
                    break;
                case FieldDataTypeEnum.String:
                    values = _stringValues;
                    break;
                default:
                    throw new Exception();
            }

            lock (_locker) {
                index = ((List<T>) values).IndexOf(value);

                if (index == -1) {
                    index = values.Count;
                    values.Add(value);
                }
            }

            Add(new FieldValueLookup(container.Id, formatField, index));
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
                var len = _items.Count;

                // Remove the field value lookup if it already exists
                for (var i = 0; i < len; i++) {
                    var item = _items[i];
                    if (item.ContainerId != containerId || item.FormatField.Id != formatFieldId) 
                        continue;

                    _items.Remove(_items[i]);
                    changed = true;
                    break;
                }
            }

            // Add the field value
            Add(container, formatField, value);

            OnValueUpdated(container, formatField, value, changed);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerId"></param>
        /// <param name="dataType"></param>
        /// <param name="fieldId"></param>
        /// <param name="isSessionId"></param>
        /// <returns></returns>
        public object Find(int containerId, FieldDataTypeEnum dataType, int fieldId, bool isSessionId) {
            var len = _items.Count;

            for (var i = 0; i < len; i++) {
                var item = _items[i];

                int id;
                if (isSessionId) {
                    if (item.FormatField == null || item.FormatField.SessionField == null)
                        continue;

                    id = item.FormatField.SessionField.Id;
                } else {
                    if (item.FormatField == null)
                        continue;

                    id = item.FormatField.Id;
                }

                if (item.ContainerId == containerId && fieldId == id) 
                    return GetValue(dataType, item.Index);
            }

            return null;
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private object GetValue(FieldDataTypeEnum dataType, int index) {
            if (dataType == FieldDataTypeEnum.DateTime)
                return _dateValues[index];
            if (dataType == FieldDataTypeEnum.Integer)
                return _intValues[index];
            if (dataType == FieldDataTypeEnum.String)
                return _stringValues[index];

            return null;
        }

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