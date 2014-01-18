using System;

namespace Medidata.Lumberjack.Core.Data.Fields.Values
{
    /// <summary>
    /// 
    /// </summary>
    public class FieldValue : IComparable<FieldValue>, IEquatable<FieldValue>
    {
        #region Private fields

        private readonly Type _type;
        private readonly LogFile _logFile;
        private readonly Entry _entry;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatField"></param>
        /// <param name="value"></param>
        private FieldValue(FormatField formatField, object value) {
            if (formatField == null)
                throw new ArgumentNullException("formatField");

            _type = formatField.Type;
            FormatField = formatField;
            Value = value;
        }

        /// <summary>
        /// Creates a new LogFieldValue instance.
        /// </summary>
        /// <param name="fieldItem">The LogFile the field value is for.</param>
        /// <param name="formatField">The FormatField of the field value</param>
        /// <param name="value">
        /// The field's value. The type of the value must be castable to the type defined
        /// by <paramref name="formatField"/>.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="fieldItem"/> or <paramref name="formatField"/> is null.
        /// </exception>
        public FieldValue(FieldItemBase fieldItem, FormatField formatField, object value)
            : this(formatField, value) {
                if (fieldItem == null)
                    throw new ArgumentNullException("fieldItem");

                if (fieldItem is LogFile) {
                    _logFile = (LogFile)fieldItem;

                } else {
                    _entry = (Entry)fieldItem;
                    _logFile = _entry.LogFile;
                }
        }

        #endregion

        #region Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public static implicit operator DateTime(FieldValue fieldValue) {
            return (DateTime)fieldValue.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public static implicit operator Int32(FieldValue fieldValue) {
            return (Int32)fieldValue.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public static implicit operator string(FieldValue fieldValue) {
            return fieldValue == null ? "" : fieldValue.ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The Entry the field value is for
        /// </summary>
        [Obsolete]
        public Entry Entry {
            get { return _entry; }
        }

        /// <summary>
        /// For FormatField of the value
        /// </summary>
        public FormatField FormatField { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public FieldItemBase Item {
            get { return (FieldItemBase) _entry ?? _logFile;  }
        }

        /// <summary>
        /// The LogFile the field value is for
        /// </summary>
        [Obsolete]
        public LogFile LogFile {
            get { return _logFile; }
        }

        /// <summary>
        /// 
        /// </summary>
        public SessionField SessionField {
            get { return FormatField.SessionField; }
        }

        /// <summary>
        /// The field's value
        /// </summary>
        public object Value { get; private set; }

        #endregion

        #region Public methods
       
        public bool ContainsComponent<T>(T component) where T : IFieldValueComponent {
            if (typeof(T) == typeof(Entry))
                return component.Equals(_entry);
            if (typeof(T) == typeof(FormatField))
                return component.Equals(FormatField);
            if (typeof(T) == typeof(LogFile))
                return component.Equals(_logFile);
            if (typeof(T) == typeof(SessionField))
                return component.Equals(SessionField);

            throw new ArgumentException("component");
        }
        
        #endregion

        #region IComparable<> implementation

        public virtual int CompareTo(FieldValue other) {
            if (_type == typeof(DateTime))
                return ((DateTime)Value).CompareTo((DateTime)other.Value);

            if (_type == typeof(Int32))
                return ((Int32)Value).CompareTo((Int32)other.Value);

            var value = (string)Value;

            return value == null ? 1 : value.CompareTo((string)other.Value);
        }

        #endregion

        #region IEquatable<> implementation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(FieldValue other) {
            return other != null
                && Item.Equals(other.Item)
                && FormatField.Equals(other.FormatField);
        }

        #endregion

        #region System.Object overrides

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            unchecked {
                var result = _logFile.Id;
                result = (result * 397) ^ (_entry != null ? _entry.Id : 0);
                result = (result * 397) ^ (FormatField != null ? FormatField.Id : 0);
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            if (_type == typeof(DateTime))
                return FormatField.FormatValue((DateTime)Value);

            if (_type == typeof(Int32))
                return FormatField.FormatValue((Int32)Value);

            return Value != null ? (string)Value : "";
        }

        #endregion
    }
}
