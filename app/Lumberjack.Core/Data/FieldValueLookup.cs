using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medidata.Lumberjack.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    public struct FieldValueLookup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerId"></param>
        /// <param name="formatField"></param>
        /// <param name="index"></param>
        public FieldValueLookup(long containerId, FormatField formatField, int index)
            : this() {
            ContainerId = containerId;
            FormatField = formatField;
            Index = index;
        }

        /// <summary>
        /// 
        /// </summary>
        public long ContainerId { get; set; }

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
