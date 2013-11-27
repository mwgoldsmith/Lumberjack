using System;
using System.Globalization;
using System.Text.RegularExpressions;
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
    public class Entry
    {
        private Regex _reRfc1123 = new Regex("^(Sun|Mon|Tue|Wed|Thu|Fri|Sat) (Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec) ([0-9]{2}) ([0-9]{2}:[0-9]{2}:[0-9]{2}) ([A-Z]{3}) ([0-9]{4})$", RegexOptions.Compiled | RegexOptions.Multiline);
        
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
            // TODO: utilize @timestampFormat of LogFormat

            DateTime timestamp;

            if (_reRfc1123.IsMatch(time))
            {
                var format = "ddd MMM dd HH:mm:ss 'UTC' yyyy";
                timestamp = DateTime.ParseExact(time, format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            }
            else
            {
                var format = "dd-MMM-yy HH:mm:ss.fff";

                if (format.Length == time.Length)
                    timestamp = DateTime.ParseExact(time, format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                else
                {
                    format = "dd-MMM-yy HH:mm:ss";
                    timestamp = format.Length == time.Length ? DateTime.ParseExact(time, format, null) : DateTime.Parse(time);
                }
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
        
        #endregion
    }
}
