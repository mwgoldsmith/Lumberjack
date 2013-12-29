using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Medidata.Lumberjack.Core.Config.Formats;
using Medidata.Lumberjack.Core.Logging;

namespace Medidata.Lumberjack.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class FormatField : FieldBase
    {
        #region Private fields

        // Saving reference to objects from which the values of this FormatField
        // were derived. Not sure if will be needed later, but keeping them for now
        private readonly SessionField _sessionField;
        private readonly FormatFieldElement _formatFieldElement;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatFieldElement"></param>
        /// <param name="sessionField"></param>
        /// <param name="id"></param>
        public FormatField(FormatFieldElement formatFieldElement, SessionField sessionField, int id) {
            if (formatFieldElement == null){
                Logger.GetInstance().Error("Failed to create FormatField: no FormatFieldElement provided");
                throw new ArgumentNullException("formatFieldElement");
            }
            

            if (sessionField == null) {
                Logger.GetInstance().Error("Failed to create FormatField for '" + formatFieldElement.Name + "': not defined as a SessionField");
                throw new ArgumentNullException("sessionField");
            }

            _sessionField = sessionField;
            _formatFieldElement = formatFieldElement;
            
            Id = id;
            DataType = sessionField.DataType;
            Display = sessionField.Display;
            Name = sessionField.Name;
            Groups = GetGroupIndexArray(formatFieldElement);
            Type = formatFieldElement.Type;

            // Sure, we could use ?:, but it really started to look like a mess
            if (String.IsNullOrEmpty(formatFieldElement.Default)) {
                Default = sessionField.Default;
            } else {
                Default = formatFieldElement.Default;   
            }

            if (formatFieldElement.IsRequiredDefault) {
                base.Required = sessionField.Required;
            } else {
                base.Required = formatFieldElement.Required;
            }

            if (formatFieldElement.IsFilterableDefault) {
                base.Filterable = sessionField.Filterable;
            } else {
                base.Filterable = formatFieldElement.Filterable;    
            }

            if (String.IsNullOrEmpty(formatFieldElement.FormatPattern)) {
                FormatPattern = sessionField.FormatPattern;
            } else {
                FormatPattern = formatFieldElement.FormatPattern;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public FormatContextEnum Type { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public int[] Groups { get; set; }

        #endregion
        
        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static Int32[] GetGroupIndexArray(FormatFieldElement formatFieldElement) {
            var indexString = formatFieldElement.GroupIndexes;

            if (String.IsNullOrEmpty(indexString))
                return null;

            var indexes = indexString.Split(',').ToList();
            var indexArray = new Int32[indexes.Count];

            for (var i = 0; i < indexes.Count && indexArray != null; i++) {
                try {
                    indexArray[i] = Int32.Parse(indexes[i], NumberStyles.Integer);
                } catch (FormatException) {
                    Logger.GetInstance().Error(String.Format("Failed to parse value '{0}' as Int32", indexes[i]));
                    indexArray = null;
                }
            }

            return indexArray;
        }

        #endregion

        #region ToString override for debugging

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return String.Format("{{ " +
                "Id = {0}, " +
                "Context = {1}, " +
                "Name = {2}, " +
                "Data Type = {3}, " +
                "Required = {4}, " +
                "Default = {5} }}",
                Id,
                Type,
                Name,
                DataType,
                Required,
                Default);
        }

        #endregion
    }
}