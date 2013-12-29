using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Logging;

namespace Medidata.Lumberjack.Core.Processors
{
    public sealed class EntryParsingEngine : EngineBase
    {
        
        #region Initializers
        
        /// <summary>
        /// 
        /// </summary>
        public EntryParsingEngine() : this(null) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public EntryParsingEngine(UserSession session) : base(session) { }

        #endregion

        protected override IEnumerable<LogFile> GetLogFilesToProcess() {
            return SessionInstance.LogFiles.ToList().FindAll(f =>
                f.EntryParseStatus == EngineStatusEnum.None
                && f.FilenameParseStatus == EngineStatusEnum.Completed);
        }

        protected override void ProcessStart(EngineState state) {
            throw new NotImplementedException();
        }

        protected override bool ProcessLog(EngineState state) {
            throw new NotImplementedException();
        }

        protected override bool ProcessComplete(EngineState state, bool success) {
            throw new NotImplementedException();
        }

        protected override void SetLogProcessStatus(LogFile logFile, EngineStatusEnum status) {
            throw new NotImplementedException();
        }

        protected override EngineStatusEnum GetLogProcessStatus(LogFile logFile) {
            throw new NotImplementedException();
        }
    }
}
