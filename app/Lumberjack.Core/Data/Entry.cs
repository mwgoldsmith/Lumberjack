using System;
using Medidata.Lumberjack.Core.Data.Fields.Values;

namespace Medidata.Lumberjack.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Entry : KeyedBase<IFieldValueComponent>, IFieldValueComponent
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

        #region KeyedBase overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override int CompareTo(IFieldValueComponent other) {
            var entry = other as Entry;

            return entry == null ? 1 : (LogFile.Id == entry.LogFile.Id)
                ? Position.CompareTo(entry.Position)
                : LogFile.CompareTo(entry.LogFile);
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
