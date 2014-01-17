using System.Xml.Serialization;

namespace Medidata.Lumberjack.Core.Data.Fields
{
    /// <summary>
    /// 
    /// </summary>
    [XmlType(IncludeInSchema = false)]
    public enum FieldDataTypeEnum
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlEnum(Name = "string")]
        String,

        /// <summary>
        /// 
        /// </summary>
        [XmlEnum(Name = "integer")]
        Integer,

        /// <summary>
        /// 
        /// </summary>
        [XmlEnum(Name = "datetime")]
        DateTime
    }
}