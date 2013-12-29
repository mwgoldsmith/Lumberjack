using System;
using System.Diagnostics;
using Medidata.Lumberjack.Core.Config.Fields;

namespace Medidata.Lumberjack.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class SessionField : FieldBase
    {
        #region Priviate fields

        // Saving reference to object from which the values of this SessionField
        // were derived. Not sure if will be needed later, but keeping them for now
        private readonly FieldElement _fieldElement;
        
        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldElement"></param>
        /// <param name="id"></param>
        public SessionField(FieldElement fieldElement, byte id) {
            _fieldElement = fieldElement;

            Id = id;
            DataType = fieldElement.DataType;
            Default = fieldElement.Default;
            Display = fieldElement.Display;
            base.Filterable = fieldElement.Filterable;
            FormatPattern = fieldElement.FormatPattern;
            Name = fieldElement.Name;
            Required = fieldElement.Required;

            var flags = FieldContextFlags.None;

            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (var fieldContextElement in fieldElement.FieldContexts) {
                flags |= (FieldContextFlags)Enum.Parse(typeof(FieldContextFlags), fieldContextElement.Type.ToString());
            }

            // ReSharper restore LoopCanBeConvertedToQuery
            ContextFlags = flags;
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public FieldContextFlags ContextFlags { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public byte Id { get; private set; }

        #endregion

        #region ToString override for debugging

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return String.Format("{{ " +
                "Id = {0}, " +
                "Context Flags = {1}, " +
                "Name = {2}, " +
                "Data Type = {3}, " +
                "Required = {4}, " +
                "Default = {5} }}",
                Id,
                ContextFlags,
                Name,
                DataType,
                Required,
                Default);
        }

        #endregion
    }
}
