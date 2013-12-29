using System;
using System.Collections.Generic;
using Medidata.Lumberjack.Core.Data;

namespace Medidata.Lumberjack.Core.Processing
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class FileParsingEngine : EngineBase
    {
        private IEnumerable<SessionFormat> _sessionFormats;

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<LogFile> GetLogFilesToProcess() {
            return SessionInstance.LogFiles.ToList().FindAll(f => f.FilenameParseStatus == EngineStatusEnum.None);
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
                var contextFormat = format.Contexts[FormatContextEnum.Filename];

                var match = contextFormat.Regex.Match(filename);
                if (!match.Success)
                    continue;

                logFile.SessionFormat = format;

                // Parse all filename fields for this log file. If successful, no need to check
                // other formats
                if (!ParseFormatFields(logFile, logFile, contextFormat, match)) {
                    break;
                }

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
        /// <returns></returns>
        protected override void ProcessComplete(LogFile logFile, bool success) {

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
    }
}
