using System;
using System.Xml.Serialization;

namespace Medidata.Lumberjack.Core.Config.Formats
{    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlType("FormatElement")]
    public class FormatElement
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("reference", DataType = "string")]
        public string Reference { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("name", DataType = "string")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("timestampFormat", DataType = "string")]
        public string TimestampFormat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("filename", typeof(FormatContextElement))]
        public FormatContextElement FilenameContext { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("entry", typeof(FormatContextElement))]
        public FormatContextElement EntryContext { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("content", typeof(FormatContextElement))]
        public FormatContextElement ContentContext { get; set; }

        #endregion
    }
}
