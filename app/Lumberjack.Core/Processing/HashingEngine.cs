using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Medidata.Lumberjack.Core.Data;

namespace Medidata.Lumberjack.Core.Processing
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class HashingEngine : EngineBase
    {
        #region Initializers
        
        /// <summary>
        /// 
        /// </summary>
        public HashingEngine() : this(null) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public HashingEngine(UserSession session) : base(session) { }

        #endregion

        #region Base overrides

        public override bool TestIfProcessable(LogFile logFile) {
            return logFile.HashStatus == EngineStatusEnum.None;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void ProcessStart() {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="engineMetrics"></param>
        /// <returns></returns>
        protected override bool ProcessLog(LogFile logFile, ref EngineMetrics engineMetrics) {
            var success = false;

            logFile.HashStatus = EngineStatusEnum.Processing;
            
            var hash = ComputeHash(logFile.FullFilename);
            if (hash != null) {
                logFile.Md5Hash = hash;

                success = true;
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
            logFile.ProcessTimeElapse[ProcessTypeEnum.Hash] = timeElapsed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="status"></param>
        protected override void SetLogProcessStatus(LogFile logFile, EngineStatusEnum status) {
            logFile.HashStatus = status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <returns></returns>
        protected override EngineStatusEnum GetLogProcessStatus(LogFile logFile) {
            return logFile.HashStatus;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename">Filename of the file to calculate the hash of.</param>
        /// <returns>If successful, the MD5 hash string; otherwise, null.</returns>
        private string ComputeHash(string filename) {
            try {
                using (var md5 = MD5.Create())
                using (var fs = File.OpenRead(filename)) {
                    return BitConverter.ToString(md5.ComputeHash(fs)).Replace("-", "").ToLower();
                }
            } catch (Exception ex) {
                OnError("An exception occured when hashing the file \"" + filename + "\"", ex);
            }

            return null;
        }

        #endregion
    }
}
