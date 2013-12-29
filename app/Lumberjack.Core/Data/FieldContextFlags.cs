using System;

namespace Medidata.Lumberjack.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum FieldContextFlags
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        Filename = 1,

        /// <summary>
        /// 
        /// </summary>
        Entry = 2,

        /// <summary>
        /// 
        /// </summary>
        Content = 4
    }
}
