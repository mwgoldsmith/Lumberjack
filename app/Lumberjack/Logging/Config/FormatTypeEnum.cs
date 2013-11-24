using System.Xml.Serialization;

namespace Medidata.Lumberjack.Logging.Config
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [XmlType(IncludeInSchema = false)]
    public enum FormatTypeEnum
    {
        None,
        [XmlEnum(Name = "filename")]
        Filename,
        [XmlEnum(Name = "entry")]
        Entry,
        [XmlEnum(Name = "content")]
        Content
    }

}
