using System;

namespace Medidata.Lumberjack.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class KeyedBase<T> : IEquatable<T>, IKeyedItem
        where T : class, IKeyedItem
    { 
        #region Initializers

        /// <summary>
        /// Creates a new FieldKeyedBase instance and acquires a unique
        /// ID value from the key generator.
        /// </summary>
        protected KeyedBase() {
            Id = KeyGenerator.GetNextId();
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// ID of the keyed object
        /// </summary>
        public int Id { get; private set; }

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
