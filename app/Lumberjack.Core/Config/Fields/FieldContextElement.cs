using System;
using System.Xml.Serialization;
using Medidata.Lumberjack.Core.Data.Formats;

namespace Medidata.Lumberjack.Core.Config.Fields
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlType("FieldContextElement")]
    public class FieldContextElement
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("name")]
        public FormatContextEnum Type { get; set; }

        #endregion
    }
}
