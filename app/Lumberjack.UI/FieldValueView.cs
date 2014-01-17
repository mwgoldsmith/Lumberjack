using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Medidata.Lumberjack.Core;
using Medidata.Lumberjack.Core.Collections;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Data.Fields.Values;

namespace Medidata.Lumberjack.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class FieldValueView<TCollection, TItem> : BindingList<TItem>, IBindingList, ITypedList
        where TCollection : IValueItemCollection<TItem>
        where TItem : KeyedBase<IFieldValueComponent>
    {
        #region Nested structs

        #region FieldValueViewRow

        /// <summary>
        /// 
        /// </summary>
        public sealed class FieldValueViewRow
        {
            #region Initializers

            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            /// <param name="fieldValues"></param>
            public FieldValueViewRow(TItem item, List<IFieldValue> fieldValues) {
                Item = item;
                FieldValues = fieldValues;
            }

            #endregion

            #region Properties

            /// <summary>
            /// 
            /// </summary>
            public TItem Item { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            public List<IFieldValue> FieldValues { get; private set; }
            
            #endregion
        }

        #endregion

        #region ObjectPropertyComparer

        /// <summary>
        /// Summary description for ObjectComparer.
        /// </summary>
        private sealed class ObjectPropertyComparer : IEqualityComparer<TItem>, IComparer
        {
            #region Private fields

            // ReSharper disable StaticFieldInGenericType
            private static readonly ObjectPropertyComparer _default = new ObjectPropertyComparer();
            // ReSharper restore StaticFieldInGenericType

            private readonly string _propertyName;

            #endregion

            #region Initializers

            /// <summary>
            /// 
            /// </summary>
            private ObjectPropertyComparer() :this(null) {
            }

            /// <summary>
            /// Provides Comparison opreations.
            /// </summary>
            /// <param name="propertyName">The property to compare</param>
            public ObjectPropertyComparer(string propertyName) {
                _propertyName = propertyName;
            }

            #endregion

            #region Properties

            /// <summary>
            /// 
            /// </summary>
            internal static ObjectPropertyComparer Default {
                get { return _default; }
            }

            #endregion

            #region IComparer implementation

            /// <summary>
            /// Compares 2 objects by their properties, given on the constructor
            /// </summary>
            /// <param name="x">First value to compare</param>
            /// <param name="y">Second value to compare</param>
            /// <returns></returns>
            public int Compare(object x, object y) {
                object a;
                object b;

                if (_propertyName != null) {
                    a = x.GetType().GetProperty(_propertyName).GetValue(x, null);
                    b = y.GetType().GetProperty(_propertyName).GetValue(y, null);
                } else {
                    a = ((TItem)x).Id;
                    b = ((TItem)y).Id;
                }

                if (a != null && b == null)
                    return 1;

                if (a == null && b != null)
                    return -1;

                return a == null ? 0 : ((IComparable)a).CompareTo(b);
            }

            #endregion

            #region IEqualityComparer<> implementation

            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(TItem x, TItem y) {
                return x.Id == y.Id;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(TItem obj) {
                return obj.Id;
            }

            #endregion
        }
        
        #endregion

        #endregion

        #region Constants

        private const int DefaultPerPage = 500;
        private const int BufferSize = 1024*8;

        #endregion

        #region Private fields

        // ReSharper disable StaticFieldInGenericType
        internal static ListChangedEventArgs _resetEventArgs = new ListChangedEventArgs(ListChangedType.Reset, -1);
        // ReSharper restore StaticFieldInGenericType

        private readonly Dictionary<TItem, FieldValueViewRow> _rowViewBuffer = new Dictionary<TItem, FieldValueViewRow>(ObjectPropertyComparer.Default);

        private readonly EntryCollection _sessionEntries;
        private readonly SessionFieldCollection _sessionFields;
        private readonly FormatFieldCollection _formatFields;
        private readonly FieldValueCollection _fieldValues;

        private readonly SessionFieldCollection _itemFields;
        
        private int _pageSize;
        private int _pageIndex;
        private bool _isSorted;
        private bool _isListening;
        private PropertyDescriptor _sortProperty;
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;
        private DataViewRowState _recordStates = DataViewRowState.CurrentRows;

        //private static readonly CaseInsensitiveComparer _objectCompare = new CaseInsensitiveComparer();

        //private readonly object _locker = new object();

        //private SessionField _sortField;
        //private SortOrder _sortOrder;
        //private List<SessionField> _viewFields;
        //private List<Entry> _entries;
        //private List<EntryRow> _view;
        //private List<IFieldValue> _unfilteredValues;

        //private int _pageIndex;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        [Obsolete]
        internal FieldValueView()
            : this(null) {

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public FieldValueView(UserSession session) {
            _itemFields = new SessionFieldCollection(session);

            _sortDirection = ListSortDirection.Ascending;
            _pageSize = DefaultPerPage;

            _sessionEntries = session.Entries;
            _sessionFields = session.SessionFields;
            _formatFields = session.FormatFields;
            _fieldValues = session.FieldValues;

            RegisterListeners();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public SessionFieldCollection Fields {
            get { return _itemFields; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int PageIndex {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int PageSizeSize {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public string[] RowFields {
        //    get {
        //        lock (_locker)
        //            return new List<string>(_viewFields.Select(f => f.Name)).ToArray();
        //    }

        //    set {
        //        lock (_locker) {
        //            _viewFields = new List<SessionField>(value.Length);
        //            foreach (var s in value)
        //                _viewFields.Add(_session.SessionFields.Find(s));

        //           // Refresh();
        //        }
        //    }
        //}

        #endregion

        //#region Indexers

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="index"></param>
        ///// <returns></returns>
        //public EntryRow this[int index] {
        //    [DebuggerStepThrough]
        //    get { return _view[index]; }
        //}

        //#endregion

        //#region Public methods

        ///// <summary>
        ///// 
        ///// </summary>
        //public bool Refresh() {
        //    bool dirty;

        //    _entries = _session.Entries.ToList();
        //    if (_sortField != null)
        //        Sort(_sortField, _sortOrder);

        //    var index = EntriesPerPage*_pageIndex;
        //    var count = Math.Min(_entries.Count - index, EntriesPerPage);

        //    dirty = _view == null || _view.Count != count;
        //    if (!dirty) {

        //        for (var i = index; i < index + count; i++)
        //            if (_view[i - index].Entry.Id != _entries[i].Id) {
        //                dirty = true;
        //                break;
        //            }
        //    }

        //    if (dirty)
        //        PopulatePageRows(index, count);

        //    return dirty;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sessionField"></param>
        ///// <param name="order"></param>
        //public void Sort(SessionField sessionField, SortOrder order) {
        //    /* Lock for the entire duration.
        //     * If not, since the inner-list isn't being sorted in place, any items
        //     * added or removed while sorting would be lost.
        //     * 
        //     * Performance might be a problem here... if so, find another way to sort.
        //     */

        //    _sortField = sessionField;
        //    _sortOrder = order;

        //    lock (_locker) {
        //        var len = _entries.Count;
        //        var values = new EntryValuePair[len];

        //        // Create array of Entry to Field Value pairs to sort
        //        for (var i = 0; i < len; i++) {
        //            var entry = _entries[i];
        //            var value = _session.FieldValues.Find(null, entry, sessionField);
        //            values[i] = new EntryValuePair(entry, value);
        //        }

        //        // Sort the pairs based on the field's data type
        //        Array.Sort(values, GetValueComparer(sessionField.Type, order));

        //        // Create a new list with the entries in sorted order
        //        var items = new List<Entry>(len);
        //        for (var i = 0; i < len; i++)
        //            items.Add(values[i].Key);

        //        // Replace the internal list with the new, sorted one
        //        _entries = items;
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public List<EntryRow> ToList() {
        //    lock (_locker)
        //        return new List<EntryRow>(_view );
        //}

        //#endregion

        //#region Private methods

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="index"></param>
        ///// <param name="count"></param>
        //private void PopulatePageRows(int index, int count) {
        //    var entries = _entries.GetRange(index, count);

        //    // Create list if SessionField IDs for each view field
        //    //var viewFieldIds = _viewFields.Select(x => x.Id).ToList();

        //    // Determine all FormatFields for which the value would need to be loaded that are
        //    // also a view field
        //    var unfilteredFields = _session.FormatFields
        //                                   .FindAll(x => !x.Filterable && x.Type == typeof (string) && _viewFields.Contains(x.SessionField));

        //    _view = new List<EntryRow>(count);

        //    var loadedValues = LoadUnfilteredValues(entries, unfilteredFields);

        //    for (var i = index; i < count; i++) {
        //        var entry = _entries[i];
        //        var values = _session.FieldValues.FindAll(entry);

        //        // Filter the field values to those linked to a view field; add entry row to view
        //        values = values.FindAll(x => _viewFields.Contains(x.FormatField.SessionField));
        //        values.AddRange(loadedValues.Where(value => value.Entry.Equals(entry)));

        //        _view.Add(new EntryRow(entry, values));
        //    }

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="allEntries"></param>
        ///// <param name="formatFields"></param>
        ///// <returns></returns>
        //private static List<EntryFieldValue> LoadUnfilteredValues(IEnumerable<Entry> allEntries, IEnumerable<FormatField> formatFields) {
        //    var items = new Dictionary<LogFile, List<Entry>>();
        //    var sessionFormats = formatFields.Select(x => x.SessionFormat).ToList();
        //    var result = new List<EntryFieldValue>();

        //    foreach (var entry in allEntries) {
        //        var logFile = entry.LogFile;

        //        if (!sessionFormats.Contains(logFile.SessionFormat) )
        //            continue;

        //        List<Entry> list;
        //        if (!items.TryGetValue(logFile, out list)) {
        //            list = new List<Entry>();
        //            items.Add(logFile,list);
        //        }

        //        list.Add(entry);
        //    }

        //    foreach (var k in items) {
        //        var entries = k.Value;

        //        // Entries are sorted in ascending order by:
        //        //   logfile's size
        //        //   entry's position
        //        entries.Sort();

        //        var fieldValues = LoadEntryValues(k.Key, entries);
        //        if (fieldValues == null)
        //            return null;

        //        result.AddRange(fieldValues);
        //    }

        //    return result;
        //} 

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="logFile"></param>
        ///// <param name="entries"></param>
        ///// <returns></returns>
        //private static IEnumerable<EntryFieldValue> LoadEntryValues(LogFile logFile, List<Entry> entries) {
        //    var result = new List<EntryFieldValue>();
        //    long start = 0;
        //    long end = 0;

        //    using (var fs = new FileStream(logFile.FullFilename, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, FileOptions.SequentialScan))
        //    using (var sr = new StreamReader(fs)) {
        //        var buffer = new Char[BufferSize];

        //        while (entries.Count > 0) {
        //            // For the remaining entries, determine the most which can be read in a single call
        //            var bufferedEntries = GetNextBufferedEntries(entries, ref start, ref end);

        //            // Seek to position of first entry and read the block of text
        //            if (start > 0) {
        //                sr.DiscardBufferedData();
        //                sr.BaseStream.Seek(start, SeekOrigin.Begin);
        //            }

        //            var count = sr.Read(buffer, 0, (int)(end - start));
        //            var text = new string(buffer, 0, count);

        //            // Extract each entry from the block; parse all fields
        //            for (var i = 0; i < bufferedEntries.Count; i++) {
        //                var entry = bufferedEntries[i];
        //                var view = text.Substring((int) (entry.Position - start), entry.Length);

        //                var fieldValues = FieldValueFactory.MatchEntryValues(entry, FormatContextEnum.Entry, view, v => !v.FormatField.Filterable);
        //                if (fieldValues == null)
        //                    throw new Exception();

        //                result.AddRange(fieldValues);
        //                entries.Remove(entry);
        //            }
        //        }
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// Determines the maxiumum number of entries that can be retrieved through a single
        ///// read operation given the current buffer size. Start position of the the first entry
        ///// and end position of the last entry are also calculated.
        ///// </summary>
        ///// <param name="entries"></param>
        ///// <param name="start"></param>
        ///// <param name="end"></param>
        ///// <returns></returns>
        //private static List<Entry> GetNextBufferedEntries(IList<Entry> entries, ref long start, ref long end) {
        //    var result = new List<Entry>();

        //    for (var i = 0; i < entries.Count; i++) {
        //        var entry = entries[i];

        //        if (i == 0) {
        //            start = entry.Position;
        //        } else {
        //            if ((entry.Position + entry.Length - start) > BufferSize)
        //                break;
        //        }

        //        end = entry.Position + entry.Length;

        //        result.Add(entry);
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="order"></param>
        ///// <returns></returns>
        //private static Comparison<EntryValuePair> GetValueComparer(Type type, SortOrder order) {
        //    var direction = order == SortOrder.Ascending ? 1 : -1;

        //    if (type == typeof(DateTime))
        //        return (x, y) => _objectCompare.Compare((DateTime)x.Value, (DateTime)y.Value) * direction;

        //    if (type == typeof(Int32))
        //        return (x, y) => _objectCompare.Compare((Int32)x.Value, (Int32)y.Value) * direction;

        //    return (x, y) => _objectCompare.Compare(x.Value, y.Value) * direction;
        //}

        //#endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        private void RegisterListeners() {
            //_sessionEntries.ItemAdded += Entries_ItemAdded;
            //_sessionEntries.ItemRemoved += Entries_ItemRemoved;
            //_sessionEntries.ItemUpdated += Entries_ItemUpdated;
            _isListening = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UnregisterListeners() {
            //_sessionEntries.ItemAdded -= Entries_ItemAdded;
            //_sessionEntries.ItemRemoved -= Entries_ItemRemoved;
            //_sessionEntries.ItemUpdated -= Entries_ItemUpdated;
            _isListening = false;
        }

        #endregion

        #region Event handlers

        void Entries_ItemUpdated(object source, CollectionItemEventArgs<TItem> e) {
            throw new NotImplementedException();
        }

        void Entries_ItemRemoved(object source, CollectionItemEventArgs<TItem> e) {
            throw new NotImplementedException();
        }

        void Entries_ItemAdded(object source, CollectionItemEventArgs<TItem> e) {
            throw new NotImplementedException();
        }
        
        #endregion

        #region IBindingList implementation

        private ListChangedEventHandler _onListChanged;

        #region Properties

        /// <summary>
        /// Gets whether you can add items to the list using AddNew().
        /// </summary>
        bool IBindingList.AllowNew {
            get { return false; }
        }

        /// <summary>
        /// Gets whether you can update items in the list.
        /// </summary>
        bool IBindingList.AllowEdit {
            get { return false; }
        }

        /// <summary>
        /// Gets whether you can remove items from the list, using Remove(System.Object)
        /// or RemoveAt(System.Int32).
        /// </summary>
        bool IBindingList.AllowRemove {
            get { return false; }
        }

        /// <summary>
        /// Gets whether a ListChanged event is raised when the list changes or an item
        /// in the list changes.
        /// </summary>
        bool IBindingList.SupportsChangeNotification {
            get { return true; }
        }

        /// <summary>
        /// Gets whether the list supports searching using the
        /// Find(PropertyDescriptor,System.Object) method.
        /// </summary>
        bool IBindingList.SupportsSearching {
            get { return true; }
        }

        /// <summary>
        /// Gets whether the list supports sorting.
        /// </summary>
        bool IBindingList.SupportsSorting {
            get { return true; }
        }

        /// <summary>
        /// Gets whether the items in the list are sorted.
        /// </summary>
        bool IBindingList.IsSorted {
            get { return _isSorted; }
        }

        /// <summary>
        /// Gets the PropertyDescriptor that is being used for sorting.
        /// </summary>
        PropertyDescriptor IBindingList.SortProperty {
            get { return _sortProperty; }
        }

        /// <summary>
        /// Gets the direction of the sort.
        /// </summary>
        ListSortDirection IBindingList.SortDirection {
            get { return _sortDirection; }
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Occurs when the list changes or an item in the list changes.
        /// </summary>
        event ListChangedEventHandler IBindingList.ListChanged {
            add {
                _onListChanged += value;
            }
            remove {
                _onListChanged -= value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a new item to the list.
        /// </summary>
        /// <returns>The item added to the list.</returns>
        object IBindingList.AddNew() {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Adds the PropertyDescriptor to the indexes used for searching.
        /// </summary>
        /// <param name="property">
        /// The PropertyDescriptor to add to the indexes used for searching.
        /// </param>
        void IBindingList.AddIndex(PropertyDescriptor property) {
            _isSorted = true;
            _sortProperty = property;
        }

        /// <summary>
        /// Sorts the list based on a PropertyDescriptor and a ListSortDirection.
        /// </summary>
        /// <param name="property">The PropertyDescriptor to sort by.</param>
        /// <param name="direction">One of the ListSortDirection values.</param>
        void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
            _isSorted = true;
            _sortProperty = property;
            _sortDirection = direction;

            //Sort(new ObjectPropertyComparer(property.Name));
          //  if (direction == ListSortDirection.Descending)
            //    Reverse();
        }

        /// <summary>
        /// Returns the index of the row that has the given PropertyDescriptor.
        /// </summary>
        /// <returns>
        /// The index of the row that has the given PropertyDescriptor.
        /// </returns>
        /// <param name="property">The PropertyDescriptor to search on. </param>
        /// <param name="key">
        /// The value of the <paramref name="property"/> parameter to search for.
        /// </param>
        /// <exception cref="T:System.NotSupportedException">SupportsSearching is false.</exception>
        int IBindingList.Find(PropertyDescriptor property, object key) {
            foreach (var o in this) {
                if (Match(typeof(TItem).GetProperty(property.Name).GetValue(o, null), key))
                    return IndexOf(o);
            }

            return -1;

        }

        /// <summary>
        /// Removes the PropertyDescriptor from the indexes used for searching.
        /// </summary>
        /// <param name="property"></param>
        void IBindingList.RemoveIndex(PropertyDescriptor property) {
            _sortProperty = null;
        }

        /// <summary>
        /// Removes any sort applied using ApplySort(PropertyDescriptor,ListSortDirection).
        /// </summary>
        void IBindingList.RemoveSort() {
            _isSorted = false;
            _sortProperty = null;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected bool Match(object x, object key) {
            if (x == null || key == null)
                return x == key;

            var dataType = x.GetType();
            if (dataType != key.GetType())
                throw new ArgumentException("Objects must be of the same type");

            if (!(dataType.IsValueType || (x is string)))
                throw new ArgumentException("Objects must be a value type");

            return Comparer.Default.Compare(x, key) == 0;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="e"></param>
        //protected virtual void OnListChanged(ListChangedEventArgs e) {
        //    if (_onListChanged != null) {
        //        _onListChanged(this, e);
        //    }
        //}
        
        #endregion

        #endregion

        #region ITypedList implementation

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listAccessors"></param>
        /// <returns></returns>
        string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
            var name = "";

            if (listAccessors != null) {
                foreach (var p in listAccessors) 
                    name += p.PropertyType.Name + "_";

                name = name.TrimEnd('_');
            } else
                name = GetType().Name;

            return name;
        }
    
        /// <summary>
        /// Returns the PropertyDescriptorCollection that represents the properties on each item used to bind data.
        /// </summary>
        /// <param name="listAccessors">
        /// An array of PropertyDescriptor objects to find in the collection as bindable. This can be null.
        /// </param>
        /// <returns>
        /// The PropertyDescriptorCollection that represents the properties on each item used to bind data.
        /// </returns>
        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
            ArrayList input;

            if (listAccessors != null && listAccessors.Length > 0) {
                input = null;
            } else {
                input = new ArrayList(TypeDescriptor.GetProperties(typeof(TItem)));
            }

            return GetPropertyDescriptorCollection(input);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected PropertyDescriptorCollection GetPropertyDescriptorCollection(ArrayList properties) {
            if (properties == null || properties.Count == 0)
                return new PropertyDescriptorCollection(null);

            var output = new ArrayList();

            foreach (PropertyDescriptor p in properties) {
                if (p.Attributes.Matches(new Attribute[] { new BindableAttribute(false) }))
                    continue;

                output.Add(p);
            }

            return new PropertyDescriptorCollection((PropertyDescriptor[])output.ToArray(typeof(PropertyDescriptor)));
        }

        #endregion

        #endregion
    }

    ///// <summary>
    ///// Summary description for FakeData.
    ///// </summary>
    //public class FakeData
    //{
    //    private FakeData() { }

    //    public static FieldValueView<Entry> GetData() {
    //        var collection = new FieldValueView<Entry>();

    //        for (var i = 0; i < 10; i++) {
    //            var iCollection = new FieldValueView<Entry>();
    //            var ii = new Entry(new LogFile(@"C:\[CTMS]\log dump\ctms-davita-innovate_DBC.log", 100), 1, 1);

    //            collection.Add(ii);

    //            for (var j = 0; j < 10; j++) {
    //                var jCollection = new FieldValueView<Entry>();
    //                var jj = new Entry(new LogFile(@"C:\[CTMS]\log dump\ctms-davita-innovate_REST.log", 100), 1, 1);
                   
    //                iCollection.Add(jj);

    //                for (var k = 0; k < 10; k++) {
    //                    var kk = new Entry(new LogFile(@"C:\[CTMS]\log dump\ctms-tklresearch-production_REPORT-20130722-133859000702-2664.log", 100), 1, 1);

    //                    jCollection.Add(kk);
    //                }
    //            }
    //        }
    //        return collection;
    //    }
    //}

}