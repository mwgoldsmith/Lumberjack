using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Medidata.Lumberjack.Core.Config.Formats
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlType("FilenameContext")]
    public class FormatContextElement
    {
        #region Initializers

        /// <summary>
        /// Creates a bew ContextBase instance
        /// </summary>
        protected FormatContextElement() {
            RegexElement = new RegexElement();
            Fields = new List<FormatFieldElement>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// List of FormatFieldElements which indicate the fields that are permitted
        /// within this Context.
        /// </summary>
        [XmlElement("field", typeof(FormatFieldElement))]
        public List<FormatFieldElement> Fields { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("regex", typeof(RegexElement))]
        public RegexElement RegexElement { get; set; }

        #endregion

    }
}
