using System;
using System.Globalization;
using System.Xml.Serialization;
using Medidata.Lumberjack.Core.Logging;

namespace Medidata.Lumberjack.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class FieldBase
    {
        #region Properties

        /// <summary>
        /// If true, the field is required; otherwise, the field is not required.
        /// </summary>
        [XmlAttribute("required", DataType = "boolean")]
        public virtual bool Required { get; set; }

        /// <summary>
        /// Default value if a field value is not specified.
        /// </summary>
        [XmlAttribute("default", DataType = "string")]
        public string Default { get; set; }

        /// <summary>
        /// Field display text.
        /// </summary>
        [XmlAttribute("display", DataType = "string")]
        public string Display { get; set; }

        /// <summary>
        /// If true, the field can be filtered by value; otherwise it cannot be filtered.
        /// To allow the field to be filtered, the value will be retrieved when the log 
        /// is parsed and it will be stored in the session field value cache. 
        /// </summary>
        [XmlAttribute("filterable", DataType = "boolean")]
        public virtual bool Filterable { get; set; }

        /// <summary>
        /// Format to use when displaying the field value
        /// </summary>
        [XmlAttribute("format", DataType = "string")]
        public string FormatPattern { get; set; }

        /// <summary>
        /// Name of the field.
        /// </summary>
        [XmlAttribute("name", DataType = "string")]
        public string Name { get; set; }

        /// <summary>
        /// An XML serializable enum value indicating the data type of the field.
        /// </summary>
        [XmlAttribute("dataType")]
        public FieldDataTypeEnum DataType { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string FormatValue(Int32 value) {
            if (DataType != FieldDataTypeEnum.Integer)
                throw new InvalidCastException("Cannot format " + DataType + " as DateTime");

            return String.IsNullOrEmpty(FormatPattern)
                       ? value.ToString("##,0")
                       : value.ToString(FormatPattern);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string FormatValue(DateTime value) {
            if (DataType != FieldDataTypeEnum.DateTime)
                throw new InvalidCastException("Cannot format " + DataType + " as DateTime");

            return String.IsNullOrEmpty(FormatPattern)
                       ? value.ToString("dd-MMM-yy HH:mm:ss.fff")
                       : value.ToString(FormatPattern);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatted"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryUnformatValue(string formatted, ref DateTime value) {
            DateTime dateValue;
            var success = false;

            if (String.IsNullOrEmpty(FormatPattern)) {
                success = DateTime.TryParse(formatted, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateValue);
                if (success)
                    value = dateValue;
            } else {
                try {
                    dateValue = DateTime.ParseExact(formatted, FormatPattern, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                    value = dateValue;
                    success = true;
                } catch (FormatException) {
                    Logger.GetInstance().Error(String.Format("Failed to parse DateTime value '{0}' using pattern '{1}'", formatted, FormatPattern));
                }
            }

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatted"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryUnformatValue(string formatted, ref Int32 value) {
            var success = false;

            try {
                value = Int32.Parse(formatted, NumberStyles.Number, CultureInfo.InvariantCulture);
                success = true;
            } catch (FormatException) {
                Logger.GetInstance().Error(String.Format("Failed to parse Int32 value '{0}'", formatted));
            }

            return success;
        }

        #endregion

        #region ToString override for debugging

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return String.Format("{{ " +
                "Name = {0}, " +
                "Data Type = {1}, " +
                "Required = {2}, " +
                "Default = {3} }}",
                Name,
                DataType,
                Required,
                Default);
        }

        #endregion
    }
}