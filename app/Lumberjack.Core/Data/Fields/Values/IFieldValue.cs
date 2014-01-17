using System;

namespace Medidata.Lumberjack.Core.Data.Fields.Values
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFieldValue : IComparable<IFieldValue>
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

        #region System.Object overrides

        string ToString();

        #endregion
    }
}
