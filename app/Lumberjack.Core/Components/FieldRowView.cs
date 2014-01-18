using System.Collections.Generic;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Data.Fields.Values;

namespace Medidata.Lumberjack.Core.Components
{

    /// <summary>
    /// 
    /// </summary>
    public sealed class FieldRowView<T> where T : FieldItemBase
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldValues"></param>
        public FieldRowView(T item, List<FieldValue> fieldValues) {
            Item = item;
            FieldValues = fieldValues;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public T Item { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public List<FieldValue> FieldValues { get; private set; }

        #endregion
    }

}
