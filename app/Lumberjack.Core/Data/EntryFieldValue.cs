
namespace Medidata.Lumberjack.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class EntryFieldValue : LogFieldValue
    {
        #region Initializers

        /// <summary>
        /// Creates a new EntryFieldValue instance
        /// </summary>
        /// <param name="entry">The Entry the field value is for</param>
        /// <param name="formatField">The FormatField of the field value</param>
        /// <param name="value">
        /// The field's value. The type of the value must be castable to the type defined
        /// by <paramref name="formatField"/>.
        /// </param>
        public EntryFieldValue(Entry entry, FormatField formatField, object value) 
            : base(entry.LogFile, formatField, value) {
            Entry = entry;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The Entry the field value is for
        /// </summary>
        public override Entry Entry { get; protected set; }

        #endregion

        #region Object overrides

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            unchecked {
                return (base.GetHashCode() * 397) ^ Entry.Id;;
            }
        }

        #endregion
    }
}
