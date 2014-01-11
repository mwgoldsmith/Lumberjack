using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Medidata.Lumberjack.Core.Data.Collections
{
    public abstract class CollectionBase<T> : SessionObject, IList<T>
    {
        #region Constants

        private const int DefaultCapacity = 16;

        #endregion

        #region Private fields

        protected readonly object _locker = new object();

        protected List<T> _items;
        
        #endregion

        #region Initializers

        /// <summary>
        /// Default constructor for the class.
        /// </summary>
        protected CollectionBase()
            : this(DefaultCapacity) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        protected CollectionBase(UserSession session) 
            : this(session, DefaultCapacity) {
        
        }

        /// <summary>
        /// Constructor that sets the size of the list to the capacity number.
        /// </summary>
        /// <param name="capacity">Number of items you want the list to default to in size.</param>
        protected CollectionBase(int capacity) 
            : this(null, capacity) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="capacity"></param>
        protected CollectionBase(UserSession session, int capacity) : base(session) {
            _items = new List<T>(capacity);
            IsReadOnly = false;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// 
        /// </summary>
        public event ItemUpdatedHandler<T> ItemRemoved;
        /// <summary>
        /// 
        /// </summary>
        public event ItemAddedHandler<T> ItemAdded;
        /// <summary>
        /// 
        /// </summary>
        public event ItemRemovedHandler<T> ItemUpdated;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int Count {
            [DebuggerStepThrough]
            get { return _items.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty {
            [DebuggerStepThrough]
            get { return _items.Count == 0;  }
        }

        #endregion

        #region Indexers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual T this[int index] {
            [DebuggerStepThrough]
            get { return _items[index]; }
            [DebuggerStepThrough]
            set { _items[index] = value; }
        }

        #endregion

        #region IList implementation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item) {
            _items.Add(item);

            OnItemAdded(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear() {
            var items = ToArray();

            _items.Clear();

            OnItemRemoved(items);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item) {
            return _items.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex) {
            _items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        public IEnumerator<T> GetEnumerator() {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item) {
            return _items.IndexOf(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item) {
            _items.Insert(index, item);

            OnItemAdded(item);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item) {
            var result = _items.Remove(item);

            OnItemRemoved(item);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index) {
            var item = _items[index];

            _items.RemoveAt(index);

            OnItemRemoved(item);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public void Add(T[] items) {
            _items.AddRange(items);

            OnItemAdded(items);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Any(Func<T, bool> predicate) {
            var items = ToArray();

            // Make all calls to predicate outside of lock
            for (var i = 0; i < items.Length; i++) {
                if (predicate(items[i]))
                    return true;
            }

            return false;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public void Remove(T[] items) {
            _items.Remove(items);

            OnItemRemoved(items);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        public T[] ToArray() {
            var count = _items.Count;
            var items = new T[count];

            for (var i = 0; i < count; i++)
                items[i] = _items[i];

            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        public List<T> ToList() {  
            return (new List<T>(_items));
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        [DebuggerStepThrough]
        protected virtual void OnItemAdded(T item) {
            OnItemAdded(new[] { item });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        [DebuggerStepThrough]
        protected virtual void OnItemAdded(T[] items) {
            if (ItemAdded != null) {
                ItemAdded(this, new CollectionItemEventArgs<T>(items)); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        [DebuggerStepThrough]
        protected virtual void OnItemRemoved(T item) {
            OnItemRemoved(new[] { item });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        [DebuggerStepThrough]
        protected virtual void OnItemRemoved(T[] items) {
            if (ItemRemoved != null) {
                ItemRemoved(this, new CollectionItemEventArgs<T>(items));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        [DebuggerStepThrough]
        protected virtual void OnItemUpdated(T item) {
            OnItemUpdated(new[] { item });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        [DebuggerStepThrough]
        protected virtual void OnItemUpdated(T[] items) {
            if (ItemUpdated != null) {
                ItemUpdated(this, new CollectionItemEventArgs<T>(items));
            }
        }

        #endregion

        #region ToString override for debugging

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        public override string ToString() {
            return "{ Count: " + _items.Count + " }";
        }

        #endregion
    }
}
