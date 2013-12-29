using System.Xml.Serialization;

namespace Medidata.Lumberjack.Core.Config.Formats
{
    /// <summary>
    /// 
    /// </summary>
    public class RegexElement
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("caseInsensitive", DataType = "boolean")]
        public bool CaseInsensitive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("singleline", DataType = "boolean")]
        public bool Singleline { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("multiline", DataType = "boolean")]
        public bool Multiline { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlText]
        public string Pattern { get; set; }

        #endregion
    }
}
