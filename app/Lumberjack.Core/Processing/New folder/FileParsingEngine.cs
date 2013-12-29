using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Logging;

namespace Medidata.Lumberjack.Core.Processors
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class FileParsingEngine : EngineBase
    {
        
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
            return new LogFile[0];
          //  SessionInstance.LogFiles.ToList().FindAll(f => f.FilenameParseStatus == EngineStatusEnum.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        protected override void ProcessStart(EngineState state) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override bool ProcessLog(EngineState state) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        protected override bool ProcessComplete(EngineState state, bool success) {
            throw new NotImplementedException();
        }

        protected override void SetLogProcessStatus(LogFile logFile, EngineStatusEnum status) {
            logFile.FilenameParseStatus = status;
        }

        protected override EngineStatusEnum GetLogProcessStatus(LogFile logFile) {
            return logFile.FilenameParseStatus;
        }

        #endregion
    }
}
