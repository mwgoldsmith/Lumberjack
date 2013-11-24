using System;
using System.Xml.Serialization;

namespace Medidata.Lumberjack.Logging.Config.Formats
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlType("Field")]
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
        [XmlText]
        public string GroupIndexes { get; set; }

        #endregion
    }
}
