using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Medidata.Lumberjack.Core.Data.Formats;
using Medidata.Lumberjack.Core.Logging;

namespace Medidata.Lumberjack.Core.Data.Fields.Values
{
    public static class FieldValueFactory
    {
        #region Private fields

        private static readonly object _locker = new object();

        private static Logger _logger;
        private static readonly List<string> _stringValues;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        static FieldValueFactory() {
            _stringValues = new List<string>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        private static Logger Logger {
            get { return _logger ?? (_logger = Logger.GetInstance()); }
        }

        #endregion

        #region Public methods
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entry"></param>
        /// <param name="formatField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FieldValue CreateLogValue<T>(FieldItemBase entry, FormatField formatField, T value) {
            var dataType = formatField.Type;

            // If the value is a string but the field is another data type, attempt to
            // convert it to the required data type
            if (typeof(T) == typeof(string) && dataType != typeof(string)) {
                if (dataType == typeof(DateTime)) {
                    DateTime dateValue;

                    if (formatField.TryUnformatValue((string)(object)value, out dateValue))
                        return CreateLogValue(entry, formatField, dateValue);
                } else if (dataType == typeof(Int32)) {
                    Int32 intValue;

                    if (formatField.TryUnformatValue((string)(object)value, out intValue))
                        return CreateLogValue(entry, formatField, intValue);
                }

                return null;
            }

            object valueRef = value;
            if (dataType == typeof(DateTime) && typeof(T) != typeof(DateTime))
                return null;
            if (dataType == typeof(Int32) && typeof(T) != typeof(Int32))
                return null;

            if (dataType == typeof(string)) {
                lock (_locker) {
                    if (_stringValues.IndexOf((string)valueRef) == -1)
                        _stringValues.Add((string)valueRef);
                }
            }

            return new FieldValue(entry, formatField, valueRef);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldItem"></param>
        /// <param name="formatContext"></param>
        /// <param name="text"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<FieldValue> MatchFieldValues(FieldItemBase fieldItem, FormatContextEnum formatContext, string text, Func<FieldValue, bool> predicate) {
            var entry = fieldItem is Entry ? (Entry) fieldItem : null;
            var logFile = entry != null ? entry.LogFile : (LogFile)fieldItem;

            var result = new List<FieldValue>();

            if (Logger.IsTraceEnabled)
                Logger.Trace("FVF-MFV.STRING", "Enter");

            var context = logFile.SessionFormat.Contexts[formatContext];
            var matches = context.Regex.Matches(text);

            foreach (Match match in matches) {
                if (!match.Success)
                    continue;

                var fieldValues = MatchFieldValues(fieldItem, formatContext, match, predicate);
                if (fieldValues == null) {
                    result = null;
                    break;
                }

                result.AddRange(fieldValues);
            }

            if (Logger.IsTraceEnabled)
                Logger.Trace("FVF-MFV.STRING", "Exit : " + (result != null ? result.Count : -1));

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldItem"></param>
        /// <param name="formatContext"></param>
        /// <param name="match"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<FieldValue> MatchFieldValues(FieldItemBase fieldItem, FormatContextEnum formatContext, Match match, Func<FieldValue, bool> predicate) {
            var entry = fieldItem is Entry ? (Entry)fieldItem : null;
            var logFile = entry != null ? entry.LogFile : (LogFile)fieldItem;

            var result = new List<FieldValue>();

            if (Logger.IsTraceEnabled)
                Logger.Trace("FVF-MFV.MATCH", "Enter");

            var context = logFile.SessionFormat.Contexts[formatContext];

            foreach (var field in context.Fields) {
                string value = null;

                if (result == null)
                    break;

                var groups = field.Groups;
                if (groups == null)
                    continue;

                // Take the first capture which was successful
                for (var j = 0; j < groups.Length; j++) {
                    if (!match.Groups[groups[j]].Success)
                        continue;

                    value = match.Groups[groups[j]].ToString();
                    break;
                }

                // The field 'CONTENT' is a special case. Get all field values within the matched field text
                // using thr Content context. Prevent recursion and ignore if already in Content
                if (field.Name.Equals("CONTENT") && formatContext != FormatContextEnum.Content) {
                    var fieldValues = MatchFieldValues(fieldItem, FormatContextEnum.Content, value, predicate);
                    if (fieldValues == null)
                        result = null;
                    else
                        result.AddRange(fieldValues);

                    continue;
                }

                // Check if field was not found but has default value
                if (value == null && field.Default != null)
                    value = field.Default;

                if (value == null)
                    continue;

                var fieldValue = CreateLogValue(formatContext == FormatContextEnum.Filename ? logFile : (FieldItemBase)entry, field, value);

                if (predicate(fieldValue))
                    result.Add(fieldValue);
            }

            if (Logger.IsTraceEnabled)
                Logger.Trace("FVF-MFV", "Exit : " + (result != null ? result.Count : -1));

            return result;
        }

        #endregion

    }
}
