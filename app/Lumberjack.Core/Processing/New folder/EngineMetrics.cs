using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medidata.Lumberjack.Core.Processors
{
    public class EngineMetrics
    {
        public long TotalMilliseconds { get; set; }

        public int AvgEntrySize { get; set; }

        public int TotalLogs { get; set; }

        public int ProcessedLogs { get; set; }

        public long TotalBytes { get; set; }

        public long ProcessedBytes { get; set; }

        public long ProcessedEntries { get; set; }

        public EngineMetrics Clone() {
            return (EngineMetrics)MemberwiseClone();
        }
    }
}
