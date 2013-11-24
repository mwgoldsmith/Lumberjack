using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medidata.Lumberjack.Logging;

namespace Medidata.Lumberjack
{
    public struct ScannerProgress
    {
        public Log Log { get; set; }
        public int TotalLogs { get; set; }
        public int CurrentLogIndex { get; set; }
        public long BytesRead { get; set; }
    }
}
