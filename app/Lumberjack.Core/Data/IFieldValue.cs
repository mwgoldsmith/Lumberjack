
namespace Medidata.Lumberjack.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFieldValue
    {
        #region Properties

        /// <summary>
        /// The LogFile the field value is for
        /// </summary>
        LogFile LogFile { get; }

        /// <summary>
        /// The Entry the field value is for
        /// </summary>
        Entry Entry { get; }

        /// <summary>
        /// For FormatField of the value
        /// </summary>
        FormatField FormatField { get; }

        /// <summary>
        /// The field's value
        /// </summary>
        object Value { get; }

        #endregion
    }
}
