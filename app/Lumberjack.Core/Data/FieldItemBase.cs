using Medidata.Lumberjack.Core.Data.Fields.Values;

namespace Medidata.Lumberjack.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class FieldItemBase : KeyedBase<FieldItemBase>, IFieldValueComponent
    {
        #region IEquatable<> implementation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IFieldValueComponent other) {
            return other != null && other.Id == Id;
        }

        #endregion
    }
}
