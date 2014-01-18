using System;

namespace Medidata.Lumberjack.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Entry : FieldItemBase, IComparable<Entry>
    {
        #region Initializers

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

        #endregion

        #region Properties

        private new int Id { get; set; }

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

        /// <summary>
        /// 
        /// </summary>
        [NotImplemented]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        //[NotImplemented]
        //public Dictionary<FieldEnum, EntryFieldValue> EnumValues { get; set; }

        #endregion

        #region IComparable<> implementation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Entry other) {
            return other == null ? 1 : (LogFile.Equals(other.LogFile))
                ? Position.CompareTo(other.Position)
                : LogFile.CompareTo(other.LogFile);
        }

        #endregion

        #region ToString override for debugging

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return String.Format("{{ " +
                "Id = {0}, " +
                "Position = {1}, " +
                "Length = {2}, " +
                "LogFile = {3} }}",
                Id,
                Position,
                Length,
                LogFile);
        }

        #endregion
    }
}
