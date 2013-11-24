using System;
using System.Xml.Serialization;

namespace Medidata.Lumberjack.Logging.Config.Nodes
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlType("FormatField")]
    public class NodeElement
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("stage", DataType = "string")]
        public string Stage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("cloud", DataType = "string")]
        public string Cloud { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlText]
        public string Host { get; set; }

        #endregion
    }
}
