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
        public static EntryFieldValue CreateEntryValue<T>(Entry entry, FormatField formatField, T value) {
            var dataType = formatField.Type;

            // If the value is a string but the field is another data type, attempt to
            // convert it to the required data type
            if (typeof(T) == typeof(string) && dataType != typeof(string)) {
                if (dataType == typeof(DateTime)) {
                    DateTime dateValue;

                    if (formatField.TryUnformatValue((string)(object)value, out dateValue))
                        return CreateEntryValue(entry, formatField, dateValue);
                } else if (dataType == typeof(Int32)) {
                    Int32 intValue;

                    if (formatField.TryUnformatValue((string)(object)value, out intValue))
                        return CreateEntryValue(entry, formatField, intValue);
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

            return new EntryFieldValue(entry, formatField, valueRef);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logFile"></param>
        /// <param name="formatField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static LogFieldValue CreateLogValue<T>(LogFile logFile, FormatField formatField, T value) {
            var dataType = formatField.Type;

            // If the value is a string but the field is another data type, attempt to
            // convert it to the required data type
            if (typeof(T) == typeof(string) && dataType != typeof(string)) {
                if (dataType == typeof(DateTime)) {
                    DateTime dateValue;

                    if (formatField.TryUnformatValue((string)(object)value, out dateValue))
                        return CreateLogValue(logFile, formatField, dateValue);
                } else if (dataType == typeof(Int32)) {
                    Int32 intValue;

                    if (formatField.TryUnformatValue((string)(object)value, out intValue))
                        return CreateLogValue(logFile, formatField, intValue);
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

            return new LogFieldValue(logFile, formatField, valueRef);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="formatContext"></param>
        /// <param name="text"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<EntryFieldValue> MatchEntryValues(Entry entry, FormatContextEnum formatContext, string text, Func<EntryFieldValue, bool> predicate) {
            return MatchFieldValues(entry.LogFile, entry, formatContext, text, predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="formatContext"></param>
        /// <param name="match"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<EntryFieldValue> MatchEntryValues(Entry entry, FormatContextEnum formatContext, Match match, Func<EntryFieldValue, bool> predicate) {
            return MatchFieldValues(entry.LogFile, entry, formatContext, match, predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="formatContext"></param>
        /// <param name="text"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<LogFieldValue> MatchLogValues(LogFile logFile, FormatContextEnum formatContext, string text, Func<LogFieldValue, bool> predicate) {
            return MatchFieldValues(logFile, null, formatContext, text, predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="formatContext"></param>
        /// <param name="match"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<LogFieldValue> MatchLogValues(LogFile logFile, FormatContextEnum formatContext, Match match, Func<LogFieldValue, bool> predicate) {
            return MatchFieldValues(logFile, null, formatContext, match, predicate);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="entry"></param>
        /// <param name="formatContext"></param>
        /// <param name="text"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private static List<T> MatchFieldValues<T>(LogFile logFile, Entry entry, FormatContextEnum formatContext, string text, Func<T, bool> predicate)
            where T : class {
            var result = new List<T>();

            if (Logger.IsTraceEnabled)
                Logger.Trace("FVF-MFV.STRING", "Enter");

            var context = logFile.SessionFormat.Contexts[formatContext];
            var matches = context.Regex.Matches(text);

            foreach (Match match in matches) {
                if (!match.Success)
                    continue;

                var fieldValues = MatchFieldValues(logFile, entry, formatContext, match, predicate);
                if (fieldValues == null)
                    break;

                result.AddRange(fieldValues);
            }

            if (Logger.IsTraceEnabled)
                Logger.Trace("FVF-MFV.STRING", "Exit : " + (result != null ? result.Count : -1));

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="entry"></param>
        /// <param name="formatContext"></param>
        /// <param name="match"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private static List<T> MatchFieldValues<T>(LogFile logFile, Entry entry, FormatContextEnum formatContext, Match match, Func<T, bool> predicate)
            where T : class {
            var result = new List<T>();

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
                    var fieldValues = MatchFieldValues(logFile, entry, FormatContextEnum.Content, value, predicate);
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

                var fieldValue = formatContext == FormatContextEnum.Filename
                                     ? (T)(object)CreateLogValue(logFile, field, value)
                                     : (T)(object)CreateEntryValue(entry, field, value);

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
