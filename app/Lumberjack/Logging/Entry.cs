using System;
using ProtoBuf;

namespace Medidata.Lumberjack.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public enum EntryLevel : byte
    {
        None,
        Trace,
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }

    /// <summary>
    /// 
    /// </summary>
    [ProtoContract]
    public class Entry : ICloneable
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entryLength"></param>
        /// <param name="time"></param>
        /// <param name="level"></param>
        /// <param name="position"></param>
        public Entry(UInt16 entryLength, string time, string level, long position)
        {
            var format = "dd-MMM-yy HH:mm:ss.fff";

            DateTime timestamp;
            if (format.Length == time.Length)
                timestamp = DateTime.ParseExact(time, format, null);
            else
            {
                format = "dd-MMM-yy HH:mm:ss";
                timestamp = format.Length == time.Length ? DateTime.ParseExact(time, format, null) : DateTime.Parse(time);
            }

            Length = entryLength;
            Position = position;
            Timestamp = timestamp;
            EntryLevel = GetLevel(level);
            Processed = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public Entry()
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(1)]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(2)]
        public EntryLevel EntryLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(3)]
        public long Position { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(4)]
        public UInt16 Length { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(5)]
        public bool Processed { get; set; }

        #endregion
        
        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static EntryLevel GetLevel(string level)
        {
            switch (level.ToUpper())
            {
                case "TRACE": return EntryLevel.Trace;
                case "DEBUG": return EntryLevel.Debug;
                case "INFO": return EntryLevel.Info;
                case "WARN": return EntryLevel.Warning;
                case "ERROR": return EntryLevel.Error;
                case "FATAL": return EntryLevel.Fatal;
            }

            return EntryLevel.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }
}
