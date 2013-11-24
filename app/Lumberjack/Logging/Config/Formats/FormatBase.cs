using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Medidata.Lumberjack.Logging.Config.Formats
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class FormatBase
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        protected FormatBase()
        {
            RegexElement = new RegexElement();
            Fields = new List<FieldElement>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("regex", typeof(RegexElement))]
        public RegexElement RegexElement { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("field", typeof(FieldElement))]
        public List<FieldElement> Fields { get; set; }

        #endregion
    }
}
