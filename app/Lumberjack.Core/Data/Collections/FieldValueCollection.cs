using System;
using System.Collections.Generic;

namespace Medidata.Lumberjack.Core.Data.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class FieldValueCollection : CollectionBase<IFieldValue>
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
        public override IFieldValue this[int index] {
            get { return _items[index]; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="entry"></param>
        /// <param name="formatField"></param>
        /// <returns></returns>
        public IFieldValue Find(LogFile logFile, Entry entry, FormatField formatField) {
            if (logFile == null && entry == null)
                throw new ArgumentNullException("logFile");
            if (formatField == null)
                throw new ArgumentNullException("formatField");

            return Find(logFile, entry, formatField.Id, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="entry"></param>
        /// <param name="sessionField"></param>
        /// <returns></returns>
        public IFieldValue Find(LogFile logFile, Entry entry, SessionField sessionField) {
            if (logFile == null && entry == null)
                throw new ArgumentNullException("logFile");
            if (sessionField == null)
                throw new ArgumentNullException("sessionField");

            return Find(logFile, entry, sessionField.Id, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionField"></param>
        /// <returns></returns>
        public List<IFieldValue> FindAll(SessionField sessionField) {
            return FindAll(x => x.FormatField != null ? x.FormatField.SessionField : null, sessionField.Id);
        }

        /// <summary>
        /// Finds all existing values for the specified FormatField.
        /// </summary>
        /// <param name="formatField"></param>
        /// <returns></returns>
        public List<IFieldValue> FindAll(FormatField formatField) {
            return FindAll(x => x.FormatField, formatField.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <returns></returns>
        public List<IFieldValue> FindAll(LogFile logFile) {
            return FindAll(x => x.LogFile, logFile.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public List<IFieldValue> FindAll(Entry entry) {
            return FindAll(x => x.Entry, entry.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logFile"></param>
        /// <param name="formatField"></param>
        /// <param name="value"></param>
        public void Add<T>(LogFile logFile,  FormatField formatField, T value) {
            Add(FieldValueFactory.CreateLogValue(logFile, formatField, value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entry"></param>
        /// <param name="formatField"></param>
        /// <param name="value"></param>
        public void Add<T>(Entry entry, FormatField formatField, T value) {
            Add(FieldValueFactory.CreateEntryValue(entry, formatField, value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logFile"></param>
        /// <param name="entry"></param>
        /// <param name="formatField"></param>
        /// <param name="value"></param>
        public void Update<T>(LogFile logFile, Entry entry, FormatField formatField, T value) {
            var formatFieldId = formatField.Id;
            var changed = false;
            var id = entry != null ? entry.Id : logFile.Id;

            lock (_locker) {
                var len = _items.Count;

                // Remove the field value lookup if it already exists
                for (var i = 0; i < len; i++) {
                    var item = _items[i];
                    if (item.FormatField.Id != formatFieldId || (item.Entry == null && entry != null))
                        continue;
                    if (entry != null ? id != item.Entry.Id : id != item.LogFile.Id)
                        continue;

                    _items.Remove(_items[i]);
                    changed = true;
                    break;
                }
            }

            // Add the field value
            if (entry == null)
                Add(logFile, formatField, value);
            else 
                Add(entry, formatField, value);

            OnValueUpdated(entry, formatField, value, changed);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="entry"></param>
        /// <param name="fieldId"></param>
        /// <param name="isSessionId"></param>
        /// <returns></returns>
        private IFieldValue Find(LogFile logFile, Entry entry, int fieldId, bool isSessionId) {
            if (logFile == null && entry == null)
                throw new ArgumentNullException("logFile");

            var logId = logFile != null ? logFile.Id : entry.LogFile.Id;
            var entryId = entry != null ? entry.Id : -1;

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

                if (fieldId != id)
                    continue;

                if ((item.Entry != null) ? item.Entry.Id == entryId : item.LogFile.Id == logId)
                    return item;
                //GetValue(dataType, item.Index);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<IFieldValue> FindAll(Func<IFieldValue, IFieldValueComponent> selector, int id) {
            var result = new List<IFieldValue>();

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
        /// <param name="component"></param>
        /// <param name="formatField"></param>
        /// <param name="value"></param>
        /// <param name="changed"></param>
        private void OnValueUpdated(IFieldValueComponent component, FormatField formatField, object value, bool changed) {
            if (ValueUpdated != null) {
                ValueUpdated(this, new ValueUpdatedEventArgs(component, formatField, value, changed));
            }
        }

        #endregion

    }
}