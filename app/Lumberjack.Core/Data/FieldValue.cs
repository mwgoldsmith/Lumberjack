using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medidata.Lumberjack.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    public struct FieldValue
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="entry"></param>
        /// <param name="formatField"></param>
        /// <param name="index"></param>
        public FieldValue(LogFile logFile, Entry entry, FormatField formatField, int index)
            : this() {
            LogFile = logFile;
            Entry = entry;
            FormatField = formatField;
            Index = index;
        }

        /// <summary>
        /// 
        /// </summary>
        public LogFile LogFile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Entry Entry { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FormatField FormatField { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Index { get; set; }
    }
}
