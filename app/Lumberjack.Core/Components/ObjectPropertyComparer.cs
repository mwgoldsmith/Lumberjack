using System;
using System.Collections;
using System.Collections.Generic;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Data.Fields.Values;

namespace Medidata.Lumberjack.Core.Components
{
    internal sealed class ObjectPropertyComparer<T> : IEqualityComparer<T>, IComparer
        where T :FieldItemBase
    {
        #region Private fields

        // ReSharper disable StaticFieldInGenericType
        private static readonly ObjectPropertyComparer<T> _default = new ObjectPropertyComparer<T>();
        // ReSharper restore StaticFieldInGenericType

        private readonly string _propertyName;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        private ObjectPropertyComparer()
            : this(null) {
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
        internal static ObjectPropertyComparer<T> Default {
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
                a = ((T)x).Id;
                b = ((T)y).Id;
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
        public bool Equals(T x, T y) {
            return x.Id == y.Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(T obj) {
            return obj.Id;
        }

        #endregion
    }
}
