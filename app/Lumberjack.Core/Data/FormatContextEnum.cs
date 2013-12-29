using System.ComponentModel;
using System.Xml.Serialization;

namespace Medidata.Lumberjack.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    [XmlType(IncludeInSchema = false)]
    public enum FormatContextEnum
    {
        [XmlIgnore]
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        [XmlEnum(Name = "filename")]
        Filename = 1,

        /// <summary>
        /// 
        /// </summary>
        [XmlEnum(Name = "entry")]
        Entry = 2,

        /// <summary>
        /// 
        /// </summary>
        [XmlEnum(Name = "content")]
        Content = 3
    }
}
