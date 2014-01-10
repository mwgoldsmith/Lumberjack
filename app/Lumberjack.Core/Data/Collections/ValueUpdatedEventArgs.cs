using System;

namespace Medidata.Lumberjack.Core.Data.Collections
{
    #region Delegates

    /// <summary>
    ///
    /// </summary>
    /// <param name="source">Object which is sending the message.</param>
    /// <param name="e">Event arguments for the event handler.</param>
    public delegate void ValueUpdatedHandler(object source, ValueUpdatedEventArgs e);

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public class ValueUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="formatField"></param>
        /// <param name="value"></param>
        /// <param name="changed"></param>
        public ValueUpdatedEventArgs(IFieldValueContainer container, FormatField formatField, object value, bool changed) {
            Container = container;
            FormatField = formatField;
            Value = value;
            Changed = changed;
        }

        /// <summary>
        /// 
        /// </summary>
        public IFieldValueContainer Container { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public FormatField FormatField { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// If true, indicates the field value was changed. Otherwise, it was added.
        /// </summary>
        public bool Changed { get; private set; }
    }
}
