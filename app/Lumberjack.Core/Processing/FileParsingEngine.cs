using System.Collections.Generic;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Data.Fields;
using Medidata.Lumberjack.Core.Data.Fields.Values;
using Medidata.Lumberjack.Core.Data.Formats;

namespace Medidata.Lumberjack.Core.Processing
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class FileParsingEngine : EngineBase
    {
        #region Constants

        private const FormatContextEnum ContextType = FormatContextEnum.Filename;

        #endregion

        #region Private fields

        private IEnumerable<SessionFormat> _sessionFormats;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        public FileParsingEngine() : this(null) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public FileParsingEngine(UserSession session) : base(session) { }

        #endregion

        #region Methods overrides

        public override bool TestIfProcessable(LogFile logFile) {
            return logFile.FilenameParseStatus == EngineStatusEnum.None;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void ProcessStart() {
            _sessionFormats = SessionInstance.Formats.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="engineMetrics"></param>
        /// <returns></returns>
        protected override bool ProcessLog(LogFile logFile, ref EngineMetrics engineMetrics) {
            var filename = logFile.Filename;
            var success = false;

            logFile.FilenameParseStatus = EngineStatusEnum.Processing;
            
            // Iterate over each Format in the session, using the first one which matches
            // the filename regex to determine the format of the log
            foreach (var format in _sessionFormats) {
                var regex = format.Contexts[ContextType].Regex;

                var match = regex.Match(filename);
                if (!match.Success)
                    continue;

                logFile.SessionFormat = format;

                // Parse all filename fields for this log file. If successful, no need to check
                // other formats

                var fieldValues = FieldValueFactory.MatchFieldValues(logFile, ContextType, match, FieldValuePredicate);
                if (fieldValues == null)
                    break;

                SessionInstance.FieldValues.Add(fieldValues.ToArray());
                //if (!ParseFormatFields(logFile, null, contextFormat, match)) {
                //    break;
                // }

                success = true;
                break;
            }

            if (success) {
                engineMetrics.ProcessedLogs++;
                engineMetrics.ProcessedBytes += logFile.Filesize;
            } else {
                engineMetrics.TotalLogs--;
                engineMetrics.TotalBytes -= logFile.Filesize;
            }

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="success"></param>
        /// <param name="timeElapsed"></param>
        protected override void ProcessComplete(LogFile logFile, bool success, long timeElapsed) {
            logFile.ProcessTimeElapse[ProcessTypeEnum.Filename] = timeElapsed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="status"></param>
        protected override void SetLogProcessStatus(LogFile logFile, EngineStatusEnum status) {
            logFile.FilenameParseStatus = status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <returns></returns>
        protected override EngineStatusEnum GetLogProcessStatus(LogFile logFile) {
            return logFile.FilenameParseStatus;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        private static bool FieldValuePredicate(FieldValue fieldValue) {
            var formatField = fieldValue.FormatField;

            if (formatField.Filterable || formatField.DataType != FieldDataTypeEnum.String)
                return false;

            return true;
        }

        #endregion
    }
}
