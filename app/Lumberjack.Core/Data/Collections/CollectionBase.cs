using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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
        protected CollectionBase(UserSession session) : this(session, DefaultCapacity) {
        
        }

        /// <summary>
        /// Constructor that sets the size of the list to the capacity number.
        /// </summary>
        /// <param name="capacity">Number of items you want the list to default to in size.</param>
        protected CollectionBase(int capacity) : this(null, capacity) {
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

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int Count {
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
            get { return _items[index]; }
            set { _items[index] = value; }
        }

        #endregion

        #region IList implementation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public virtual void Add(T item) {
            _items.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Clear() {
            _items.Clear();
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
        public IEnumerator<T> GetEnumerator() {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
        public virtual void Insert(int index, T item) {
            _items.Insert(index, item);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Remove(T item) {
            return _items.Remove(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public virtual void RemoveAt(int index) {
            _items.RemoveAt(index);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entries"></param>
        public virtual void Add(T[] entries) {
            _items.AddRange(entries);
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
        public virtual void Remove(T[] items) {
            _items.Remove(items);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
        public List<T> ToList() {
            return (new List<T>(_items));
        }

        #endregion

        #region ToString override for debugging

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return "{ Count: " + _items.Count + " }";
        }

        #endregion
    }
}
