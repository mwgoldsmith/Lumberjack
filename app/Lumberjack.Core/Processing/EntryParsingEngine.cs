using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Logging;

namespace Medidata.Lumberjack.Core.Processing
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class EntryParsingEngine : EngineBase
    {
        #region Constants

        private const int BufferSize = 1024 * 8;

        #endregion
        
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
        
        #region Base overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <returns></returns>
        public override bool TestIfProcessable(LogFile logFile) {
            return logFile.EntryParseStatus == EngineStatusEnum.None && logFile.SessionFormat != null;
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

            logFile.EntryParseStatus = EngineStatusEnum.Processing;
            
            if (logFile.Filesize == 0) {
                // Log file is 0 bytes, consider this a success and go on with our life
                return true;
            }

            var state = new EngineState();
            var formatContext = logFile.SessionFormat.Contexts[FormatContextEnum.Entry];

            try {
                using (var fs = new FileStream(logFile.FullFilename, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, FileOptions.SequentialScan))
                using (var sr = new StreamReader(fs)) {
                    state.StreamReader = sr;

                    var buffer = new Char[BufferSize];
                    var entries = new List<Entry>(1024);
                    var remaining = "";

                    OnProgressChanged(engineMetrics, logFile);

                    while (!IsStopping) {
                        var count = sr.Read(buffer, 0, (int)(Math.Min(BufferSize, state.TotalBytes - state.BytesRead)));
                        if (count == 0)
                            break;

                        var text = new string(buffer, 0, count);
                        var view = String.Concat(remaining, text);
                        var lastPos = 0;
                        var matches = formatContext.Regex.Matches(view);

                        for (var i = 0; i < matches.Count && !IsStopping; i++) {
                            var match = matches[i];

                            if (!match.Success)
                                continue;

                            var position = state.BytesProcessed + state.Encoding.GetByteCount(view.Substring(0, match.Index));
                            var entry = new Entry(logFile, position, (ushort) match.Length);

                            lastPos = match.Index + match.Length - remaining.Length;

                            if (ParseFormatFields(logFile, entry, formatContext, match)) {
                                logFile.EntryStats.TotalEntries++;
                                entries.Add(entry);
                            } else {
                                OnInfo(String.Format("Failed to parse log entry at byte position {0:##,#} in file \"{1}\".", position, logFile.Filename));
                                break;
                            }
                        }

                        var bytes = state.Encoding.GetByteCount(view) - (count - lastPos);
                        state.BytesProcessed += bytes;
                        state.BytesRead += count;

                        if (lastPos < count) {
                            remaining = text.Substring(lastPos);
                        }

                        engineMetrics.ProcessedBytes += state.Encoding.GetByteCount(text);
                        AddToMetrics(entries.ToArray(), ref engineMetrics);

                        SessionInstance.Entries.Add(entries.ToArray());
                        entries.Clear();

                        OnProgressChanged(engineMetrics, logFile);

                        //len = log.Entries.Length;
                        //log.EntryStats.LastEntry = len > 0 ? log.Entries[len - 1].Timestamp : DateTime.MinValue;
                        //log.EntryStats.FirstEntry = len > 0 ? log.Entries[0].Timestamp : DateTime.MinValue;
                    }

                    sr.Close();
                }

                success = true;
            } catch (Exception ex) {
                OnError(String.Format("Failed to parse log entries for \"{0}\".", logFile.Filename), ex);
            }

            if (success) {
                engineMetrics.ProcessedLogs++;
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
            logFile.ProcessTimeElapse[ProcessTypeEnum.Entries] = timeElapsed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="status"></param>
        protected override void SetLogProcessStatus(LogFile logFile, EngineStatusEnum status) {
            logFile.EntryParseStatus = status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <returns></returns>
        protected override EngineStatusEnum GetLogProcessStatus(LogFile logFile) {
            return logFile.EntryParseStatus;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="metrics"></param>
        private void AddToMetrics(Entry[] entries, ref EngineMetrics metrics) {
            ushort size = 0;
            var len = entries.Length;

            for (var i = 0; i < len; i++)
                size += entries[i].Length;

            if (metrics.ProcessedEntries > 0) {
                metrics.AvgEntrySize = (ushort) ((metrics.AvgEntrySize + size)/(len + 1));
            } else {
                metrics.AvgEntrySize = (ushort) (size / len);
            }

            metrics.ProcessedEntries += len;
        }

        #endregion
    }
}
