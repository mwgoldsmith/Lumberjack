using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medidata.Lumberjack.Core.Data
{
    public class Entry : IFieldValueContainer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="position"></param>
        /// <param name="length"></param>
        public Entry(LogFile logFile, long position, UInt16 length) {
            LogFile = logFile;
            Length = length;
            Position = position;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UInt16 Length { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public LogFile LogFile { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public long Position { get; private set; }

        #region ToString override for debugging

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return String.Format("{{ " +
                "Id = {0}, " +
                "Length = {1}, " +
                "LogFile = {2}, " +
                "Position = {3} }}",
                Id,
                Length,
                LogFile,
                Position);
        }

        #endregion
    }
}
