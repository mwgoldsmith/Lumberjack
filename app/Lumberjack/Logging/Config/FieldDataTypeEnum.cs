using System.Xml.Serialization;

namespace Medidata.Lumberjack.Logging.Config
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [XmlType(IncludeInSchema = false)]
    public enum FieldDataTypeEnum
    {
        None,
        [XmlEnum(Name = "string")]
        String,
        [XmlEnum(Name = "int")]
        Integer,
        [XmlEnum(Name = "datetime")]
        DateTime
    }
}
