using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Medidata.Lumberjack.Core.Data.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class FieldValueCollection : CollectionBase<FieldValueLookup>
    {
        #region Private fields

        private readonly Dictionary<FieldDataTypeEnum, IList> _values;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public FieldValueCollection(UserSession session) : base(session) {
            _values = new Dictionary<FieldDataTypeEnum, IList>
                {
                    {FieldDataTypeEnum.DateTime, new List<DateTime>()},
                    {FieldDataTypeEnum.Integer, new List<Int32>()},
                    {FieldDataTypeEnum.String, new List<string>()}
                };
        }

        #endregion

        #region Indexers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override FieldValueLookup this[int index] {
            get { return _items[index]; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container">The object the value is related to. (ex: LogFile, Entry)</param>
        /// <param name="formatField"></param>
        /// <returns></returns>
        public object Find(IFieldValueContainer container, FormatField formatField) {
            if (container == null)
                throw new ArgumentNullException("container");
            if (formatField == null)
                throw new ArgumentNullException("formatField");
            
            var containerId = container.Id;
            var formatFieldId = formatField.Id;
            for (var i = 0; i < _items.Count; i++) {
                var item = _items[i];
                if (item.ContainerId == containerId && item.FormatField.Id == formatFieldId) {
                    return _values[formatField.DataType][item.Index];
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="formatField"></param>
        /// <param name="value"></param>
        public void Add<T>(IFieldValueContainer container, FormatField formatField, T value) {
            if (GetFormatFieldType(formatField.DataType) != typeof(T))
                throw new ArgumentException("Cannot add " + typeof(T) + " value for field '" + formatField.Name + "': data type is '" + formatField.DataType + "'");

            lock (_locker) {
                var values = _values[formatField.DataType];
                var index = ((List<T>)values).IndexOf(value);
                if (index == -1) {
                    index = values.Count;
                    values.Add(value);
                }

                base.Add(new FieldValueLookup(container.Id, formatField, index));
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldDataType"></param>
        /// <returns></returns>
        private static Type GetFormatFieldType(FieldDataTypeEnum fieldDataType) {
            if (fieldDataType == FieldDataTypeEnum.DateTime)
                return typeof(DateTime);
            if (fieldDataType == FieldDataTypeEnum.Integer)
                return typeof(Int32);
            if (fieldDataType == FieldDataTypeEnum.String)
                return typeof(string);

            throw new InvalidEnumArgumentException("fieldDataType");
        }
       
        #endregion

    }
}