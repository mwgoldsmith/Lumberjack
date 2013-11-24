using System;
using System.Xml.Serialization;

namespace Medidata.Lumberjack.Logging.Config.Fields
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlType("FormatField")]
    public class FieldElement
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("id", DataType = "string")]
        public string Identifier { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("required", DataType = "boolean")]
        public bool Required { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("default", DataType = "string")]
        public string Default { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("formatType")]
        public FormatTypeEnum FieldFormatType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("dataType")]
        public FieldDataTypeEnum DataType { get; set; }

        #endregion
    }
}
