
using System;

namespace Medidata.Lumberjack.Core.Data.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class FieldKeyedBase<T> : FieldBase, IKeyedItem, IComparable, IEquatable<T>, IComparable<T>
        where T : class, IKeyedItem
    {
        #region Initializers

        /// <summary>
        /// Creates a new FieldKeyedBase instance and acquires a unique
        /// ID value from the key generator.
        /// </summary>
        protected FieldKeyedBase() {
            Id = KeyGenerator.GetNextId();
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// ID of the keyed object
        /// </summary>
        public int Id { get; private set; }

        #endregion

        #region IComparable implementation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        int IComparable.CompareTo(object obj) {
            if (obj == null)
                return 1;

            var other = obj as T;
            if (other == null)
                throw new ArgumentException("Object is not an instance of " + (typeof(T).Name));

            return CompareTo(other);
        }

        #endregion

        #region IComparable<> implementation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual int CompareTo(T other) {
            return other == null ? 1 : Id.CompareTo(other.Id);
        }

        #endregion

        #region IEquatable<> implementation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(T other) {
            return other != null && other.Id == Id;
        }

        #endregion

        #region System.Object overrides

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            unchecked {
                return 397 ^ Id;
            }
        }

        #endregion
    }
}
