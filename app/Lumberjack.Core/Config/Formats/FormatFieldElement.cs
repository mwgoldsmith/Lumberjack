using System;
using System.Xml.Serialization;
using Medidata.Lumberjack.Core.Data;

namespace Medidata.Lumberjack.Core.Config.Formats
{    /// <summary>
    /// A FormatFieldElement object is used solely for the purpose of serialization.
    /// It provides the structure most suitable for layout as XML, and is used as an
    /// intermediary between the XML file and FormatField objects.
    /// 
    /// It is used to define which Fields can exist within a Format Context. Additionally,
    /// the group index value(s) are specified to indicate how to determine the value
    /// when Match() is called using the Context's regex.
    /// 
    /// Any property values set in the FormatFieldElement will override the property
    /// value of the base field definition from FieldElement, with 2 exceptions:
    ///   - Name is used to reference the Field, and thus cannot be overriden
    /// </summary>
    [Serializable]
    [XmlType("FormatFieldElement")]
    public class FormatFieldElement : FieldBase
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        public FormatFieldElement() {
            IsFilterableDefault = true;
            IsRequiredDefault = true;
        }

        #endregion

        #region Field properties which cannot be overriden
        // ReSharper disable UnusedMember.Local

        [XmlIgnore]
        private new FieldDataTypeEnum DataType { get; set; }

        [XmlIgnore]
        private new string Display { get; set; }

        // ReSharper restore UnusedMember.Local
        #endregion

        #region Public properties

        /// <summary>
        /// If true, the field can be filtered by value; otherwise it cannot be filtered.
        /// To allow the field to be filtered, the value will be retrieved when the log 
        /// is parsed and it will be stored in the session field value cache. 
        /// </summary>
        [XmlAttribute("filterable", DataType = "boolean")]
        public override bool Filterable {
            get { return base.Filterable; }
            set {
                base.Filterable = value;

                // Set a flag indicating that this property is explicitly being set. This
                // is used to differentiate between simply having its default value or if
                // it was set to false
                IsFilterableDefault = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("required", DataType = "boolean")]
        public override bool Required {
            get { return base.Required; }
            set {
                base.Required = value;

                // Set a flag indicating that this property is explicitly being set. This
                // is used to differentiate between simply having its default value or if
                // it was set to false
                IsRequiredDefault = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlText]
        public string GroupIndexes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public FormatContextEnum Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public bool IsFilterableDefault { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public bool IsRequiredDefault { get; private set; }

        #endregion
    }
}
