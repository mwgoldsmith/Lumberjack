using System;
using System.Xml.Serialization;

namespace Medidata.Lumberjack.Logging.Config.Formats
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlType("Format")]
    public class LogFormat
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("identifier", DataType = "string")]
        public string Identifier { get; set; }

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
        [XmlElement("filename", typeof(FilenameFormat))]
        public FilenameFormat Filename { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("entry", typeof(EntryFormat))]
        public EntryFormat Entry { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("content", typeof(ContentFormat))]
        public ContentFormat Content { get; set; }

        #endregion
    }
}
