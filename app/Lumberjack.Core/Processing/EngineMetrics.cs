using System;

namespace Medidata.Lumberjack.Core.Processing
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class EngineMetrics
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public long TotalMilliseconds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ushort AvgEntrySize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ProcessedLogs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long ProcessedBytes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long ProcessedEntries { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long TotalBytes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalLogs { get; set; }

        #endregion

        #region ToString override for debugging

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return String.Format("{{ " +
                "TotalMilliseconds = {0}, " +
                "AvgEntrySize = {1}, " +
                "ProcessedLogs = {2}, " +
                "ProcessedBytes = {3}, " +
                "ProcessedEntries = {4}, " +
                "TotalBytes = {5}, " +
                "TotalLogs = {6} }}",
                TotalMilliseconds,
                AvgEntrySize,
                ProcessedLogs,
                ProcessedBytes,
                ProcessedEntries,
                TotalBytes,
                TotalLogs);
        }

        #endregion
    }
}
