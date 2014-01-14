using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Medidata.Lumberjack.Core;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Data.Collections;

namespace Medidata.Lumberjack.UI
{
    public class EntryView
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
            /// <param name="entry"></param>
            /// <param name="value"></param>
            public EntryValuePair(Entry entry, object value) : this() {
                Entry = entry;
                Value = value;
            }

            /// <summary>
            /// 
            /// </summary>
            public Entry Entry { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            public object Value { get; private set; }
        }


        /// <summary>
        /// 
        /// </summary>
        public struct EntryRow
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="entry"></param>
            /// <param name="fieldValues"></param>
            public EntryRow(Entry entry, List<IFieldValue> fieldValues) : this() {
                Entry = entry;
                FieldValues = fieldValues;
            }

            /// <summary>
            /// 
            /// </summary>
            public Entry Entry { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            public List<IFieldValue> FieldValues { get; private set; }
        }


        #endregion

        #region Constants

        private const int DefaultPerPage = 500;
        private const int BufferSize = 1024 * 8;

        #endregion

        #region Private fields

        private static readonly CaseInsensitiveComparer _objectCompare = new CaseInsensitiveComparer();

        private readonly UserSession _session;
        private readonly object _locker = new object();

        private SessionField _sortField;
        private SortOrder _sortOrder;
        private List<SessionField> _viewFields;
        private List<Entry> _entries;
        private List<EntryRow> _view;
        private List<IFieldValue> _unfilteredValues;

        private int _pageIndex;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public EntryView(UserSession session) {
            _session = session;
            _viewFields = new List<SessionField>();

            EntriesPerPage = DefaultPerPage;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int EntriesPerPage { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int PageIndex {
            get { return _pageIndex; }
            set {
                lock (_locker) {
                    if (_pageIndex != value) {
                        _pageIndex = value;

                        Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] RowFields {
            get {
                lock (_locker)
                    return new List<string>(_viewFields.Select(f => f.Name)).ToArray();
            }

            set {
                lock (_locker) {
                    _viewFields = new List<SessionField>(value.Length);
                    foreach (var s in value)
                        _viewFields.Add(_session.SessionFields.Find(s));

                    Refresh();
                }
            }
        }

        #endregion

        #region Indexers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public EntryRow this[int index] {
            [DebuggerStepThrough]
            get { return _view[index]; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        public void Refresh() {
            _entries = _session.Entries.ToList();
            if (_sortField != null)
                Sort(_sortField, _sortOrder);

            var index = EntriesPerPage * _pageIndex;
            var count = Math.Min(_entries.Count - index, EntriesPerPage);

            PopulatePageRows(index, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionField"></param>
        /// <param name="order"></param>
        public void Sort(SessionField sessionField, SortOrder order) {
            /* Lock for the entire duration.
             * If not, since the inner-list isn't being sorted in place, any items
             * added or removed while sorting would be lost.
             * 
             * Performance might be a problem here... if so, find another way to sort.
             */

            _sortField = sessionField;
            _sortOrder = order;

            lock (_locker) {
                var len = _entries.Count;
                var values = new EntryValuePair[len];

                // Create array of Entry to Field Value pairs to sort
                for (var i = 0; i < len; i++) {
                    var entry = _entries[i];
                    var value = _session.FieldValues.Find(null, entry, sessionField);
                    values[i] = new EntryValuePair(entry, value);
                }

                // Sort the pairs based on the field's data type
                Array.Sort(values, GetValueComparer(sessionField.DataType, order));

                // Create a new list with the entries in sorted order
                var items = new List<Entry>(len);
                for (var i = 0; i < len; i++)
                    items.Add(values[i].Entry);

                // Replace the internal list with the new, sorted one
                _entries = items;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<EntryRow> ToList() {
            lock (_locker)
                return new List<EntryRow>(_view);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        private void PopulatePageRows(int index, int count) {
            var entries = _entries.GetRange(index, count);

            // Create list if SessionField IDs for each view field
            var viewFieldIds = _viewFields.Select(x => x.Id).ToList();

            // Determine all FormatFields for which the value would need to be loaded that are
            // also a view field
            var unfilteredFields = _session.FormatFields
                .FindAll(x => !x.Filterable && x.Type == typeof(string))
                .FindAll(x => viewFieldIds.IndexOf(x.SessionField.Id) != -1);

            _view = new List<EntryRow>(count);

            var loadedValues = LoadUnfilteredValues(entries, unfilteredFields);

            for (var i = index; i < count; i++) {
                var entry = _entries[i];
                var values = _session.FieldValues.FindAll(entry);

                // Filter the field values to those linked to a view field; add entry row to view
                values = values.FindAll(x => viewFieldIds.IndexOf(x.FormatField.SessionField.Id) != 0);
                values.AddRange(loadedValues.Where(value => value.Entry.Id == entry.Id));

                _view.Add(new EntryRow(entry, values));
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allEntries"></param>
        /// <param name="formatFields"></param>
        /// <returns></returns>
        private static List<EntryFieldValue> LoadUnfilteredValues(IEnumerable<Entry> allEntries, IEnumerable<FormatField> formatFields) {
            var items = new Dictionary<LogFile, List<Entry>>();
            var ids = formatFields.Select(x => x.SessionFormat.Id).ToList();
            var result = new List<EntryFieldValue>();

            foreach (var entry in allEntries) {
                var logFile = entry.LogFile;

                if (ids.IndexOf(logFile.SessionFormat.Id) == -1)
                    continue;

                List<Entry> list;
                if (!items.TryGetValue(logFile, out list)) {
                    list = new List<Entry>();
                    items.Add(logFile,list);
                }

                list.Add(entry);
            }

            foreach (var k in items) {
                var entries = k.Value;

                entries.Sort((x, y) => x.Position.CompareTo(y.Position));

                var fieldValues = LoadEntryValues(k.Key, entries);
                if (fieldValues == null)
                    return null;

                result.AddRange(fieldValues);
            }

            return result;
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        private static IEnumerable<EntryFieldValue> LoadEntryValues(LogFile logFile, List<Entry> entries) {
            var result = new List<EntryFieldValue>();
            long start = 0;
            long end = 0;

            using (var fs = new FileStream(logFile.FullFilename, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, FileOptions.SequentialScan))
            using (var sr = new StreamReader(fs)) {
                var buffer = new Char[BufferSize];

                while (entries.Count > 0) {
                    // For the remaining entries, determine the most which can be read in a single call
                    var bufferedEntries = GetNextBufferedEntries(entries, ref start, ref end);

                    // Seek to position of first entry and read the block of text
                    if (start > 0) {
                        sr.DiscardBufferedData();
                        sr.BaseStream.Seek(start, SeekOrigin.Begin);
                    }

                    var count = sr.Read(buffer, 0, (int)(end - start));
                    var text = new string(buffer, 0, count);

                    // Extract each entry from the block; parse all fields
                    for (var i = 0; i < bufferedEntries.Count; i++) {
                        var entry = bufferedEntries[i];
                        var view = text.Substring((int)(entry.Position - start), entry.Length);

                        var fieldValues = FieldValueFactory.MatchEntryValues(entry, FormatContextEnum.Entry, view, v => !v.FormatField.Filterable);
                        if (fieldValues == null)
                            throw new Exception();

                        result.AddRange(fieldValues);
                        entries.Remove(entry);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Determines the maxiumum number of entries that can be retrieved through a single
        /// read operation given the current buffer size. Start position of the the first entry
        /// and end position of the last entry are also calculated.
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static List<Entry> GetNextBufferedEntries(IList<Entry> entries, ref long start, ref long end) {
            var result = new List<Entry>();

            for (var i = 0; i < entries.Count; i++) {
                var entry = entries[i];

                if (i == 0) {
                    start = entry.Position;
                    end = start + entry.Length;
                } else {
                    var diff = entry.Position - end - entry.Length;
                    if (end - start + diff > BufferSize)
                        break;

                    end += diff;
                }

                result.Add(entry);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private static Comparison<EntryValuePair> GetValueComparer(FieldDataTypeEnum dataType, SortOrder order) {
            var direction = order == SortOrder.Ascending ? 1 : -1;

            if (dataType == FieldDataTypeEnum.DateTime) {
                return (x, y) => _objectCompare.Compare((DateTime)x.Value, (DateTime)y.Value) * direction;
            }

            if (dataType == FieldDataTypeEnum.Integer) {
                return (x, y) => _objectCompare.Compare((Int32)x.Value, (Int32)y.Value) * direction;
            }

            return (x, y) => _objectCompare.Compare(x.Value, y.Value) * direction;
        }

        #endregion
    }
}
