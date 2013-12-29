using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Medidata.Lumberjack.Core.Data;

namespace Medidata.Lumberjack.Core.Processors
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<LogFile> GetLogFilesToProcess() {
            return SessionInstance.LogFiles.ToList().FindAll(f => f.HashStatus == EngineStatusEnum.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        protected override void ProcessStart(EngineState state) {
            state.CurrentContext = FormatContextEnum.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override bool ProcessLog(EngineState state) {
            var log = state.LogFile;

            log.HashStatus = EngineStatusEnum.Processing;

            var hash = ComputeHash(log.FullFilename);
            if (hash == null || SessionInstance.LogFiles.ContainsHash(hash)) {
                return false;
            }

            log.Md5Hash = hash;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        protected override bool ProcessComplete(EngineState state, bool success) {
            var log = state.LogFile;
            string msg;

            if (success) {
                msg = String.Format("File \"{0}\" has been added to the session ({1} bytes, MD5 hash: {2})", log.Filename, log.Filesize, log.Md5Hash);
            } else {
                if (log.Md5Hash == null) {
                    msg = String.Format("Failed to hash file \"{0}\"; removing from the session.", log.Filename);
                } else {
                    msg = String.Format("File with hash {0} already exists in session. Removing \"{1}\".", log.Md5Hash, log.Filename);
                }

                SessionInstance.LogFiles.Remove(log);
            }

            OnInfo(msg);

            return true;
        }

        protected override void SetLogProcessStatus(LogFile logFile, EngineStatusEnum status) {
            logFile.HashStatus = status;
        }

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
