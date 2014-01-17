using System;

namespace Medidata.Lumberjack.Core.Data.Fields.Values
{
    /// <summary>
    /// 
    /// </summary>
    public class LogFieldValue : IFieldValue
    {
        #region Private fields

        private readonly Type _type;

        #endregion

        #region Initializers

        /// <summary>
        /// Creates a new LogFieldValue instance.
        /// </summary>
        /// <param name="logFile">The LogFile the field value is for.</param>
        /// <param name="formatField">The FormatField of the field value</param>
        /// <param name="value">
        /// The field's value. The type of the value must be castable to the type defined
        /// by <paramref name="formatField"/>.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="logFile"/> or <paramref name="formatField"/> is null.
        /// </exception>
        public LogFieldValue(LogFile logFile, FormatField formatField, object value) {
            if (logFile == null)
                throw new ArgumentNullException("logFile");
            if (formatField == null)
                throw new ArgumentNullException("formatField");

            _type = formatField.Type;

            LogFile = logFile;
            FormatField = formatField;
            Value = value;
        }

        #endregion

        #region Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFieldValue"></param>
        /// <returns></returns>
        public static implicit operator DateTime(LogFieldValue logFieldValue) {
            return (DateTime)logFieldValue.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFieldValue"></param>
        /// <returns></returns>
        public static implicit operator Int32(LogFieldValue logFieldValue) {
            return (Int32)logFieldValue.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFieldValue"></param>
        /// <returns></returns>
        public static implicit operator string(LogFieldValue logFieldValue) {
            return logFieldValue == null ? "" : logFieldValue.ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The LogFile the field value is for
        /// </summary>
        public LogFile LogFile { get; private set; }

        /// <summary>
        /// The Entry the field value is for
        /// </summary>
        public virtual Entry Entry { get; protected set; }

        /// <summary>
        /// For FormatField of the value
        /// </summary>
        public FormatField FormatField { get; private set; }

        /// <summary>
        /// The field's value
        /// </summary>
        public object Value { get; private set; }

        #endregion

        #region IComparable<> implementation

        public virtual int CompareTo(IFieldValue other) {
            if (_type == typeof(DateTime))
                return ((DateTime)Value).CompareTo((DateTime)other.Value);

            if (_type == typeof(Int32))
                return ((Int32)Value).CompareTo((Int32)other.Value);

            var value = (string)Value;

            return value == null ? 1 : value.CompareTo((string)other.Value);
        }

        #endregion

        #region System.Object overrides

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            unchecked {
                var result = LogFile.Id;
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
