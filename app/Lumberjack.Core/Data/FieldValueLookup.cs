using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medidata.Lumberjack.Core.Data
{
    public struct FieldValueLookup
    {
        public FieldValueLookup(long containerId, FormatField formatField, int index)
            : this() {
            ContainerId = containerId;
            FormatField = formatField;
            Index = index;
        }

        public long ContainerId { get; set; }
        public FormatField FormatField { get; set; }
        public int Index { get; set; }
    }
}
