using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ProtoBuf;

namespace Medidata.Lumberjack.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public enum LogType : byte
    {
        None,
        Access,
        Audit,
        BatchJobs,
        Billing,
        Catalina,
        Config,
        D2D,
        DbContext,
        Default,
        Email,
        Enhancers,
        Error,
        Export,
        FilePoller,
        GenerateActivities,
        Menu,
        Quartz,
        Report,
        Secure,
        Rest,
        ServiceTracker,
        SslAccess,
        SslError,
        Sync,
        Triggers,
        VeloBean,
        Velocity
    }

    /// <summary>
    /// 
    /// </summary>
    [ProtoContract]
    public class Log : ICloneable
    {
        #region Private fields

        private static readonly Regex _reRackspaceLogType = new Regex(@"^([^-_])*[-_](?:(dev|uat|training)\d?[-_])?(.*?)[-_.](?!\-\d)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static readonly Regex _reAwsLogType = new Regex(@"^ctms-([^-]+)-(?:production|innovate)[-_.](.*?)[-_.](?!\-\d)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        //private readonly List<Entry> _entries = new List<Entry>(1024);
        private DateTime _endTime = DateTime.MinValue;
        private DateTime _startTime = DateTime.MinValue;
        private Entry[] _entries = null;

        private static string _fileExt = ".tree";

        private static readonly object _locker = new object();

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        public Log()
            : this(null)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public Log(string filename)
        {
            if (filename != null)
            {
                FullFilename = Path.GetFullPath(filename);
                Filename = Path.GetFileName(filename);
                FileSize = new FileInfo(filename).Length;
                LogType = GetType(filename);
                Project = GetProject(filename);
                NodeName = GetNode(filename);
            }
            else
            {
                FullFilename = null;
                Filename = null;
                FileSize = 0;
                LogType = LogType.None;
                Project = null;
                NodeName = null;
            }

            TraceCount = 0;
            DebugCount = 0;
            InfoCount = 0;
            ErrorCount = 0;
            WarnCount = 0;
            FatalCount = 0;

            BytesParsed = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public static string SavedStateFileExt
        {
            get { return _fileExt; } 
            set { _fileExt = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(1)]
        public string FullFilename { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(2)]
        public string Filename { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(3)]
        public long FileSize { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(4)]
        public string Md5Hash { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(5)]
        public LogType LogType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(6)]
        public string Project { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(7)]
        public string NodeName { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(8)]
        public DateTime StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(9)]
        public DateTime EndTime {
            get { return _endTime; }
            set { _endTime = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(10)]
        public Entry[] Entries
        {
            get { lock (_locker) return _entries; }
            set { lock(_locker) _entries = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(11)]
        public int TraceCount { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(12)]
        public int DebugCount { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(13)]
        public int InfoCount { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(14)]
        public int WarnCount { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(15)]
        public int ErrorCount { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(16)]
        public int FatalCount { get; private set; }

        #endregion

        // TODO: Move to subclass (ParsedLog)
        public long BytesParsed { get; set; }

        // TODO: Move to subclass (ParsedLog)
        public StreamReader Stream { get; set; }

        // TODO: Move to subclass (ParsedLog)
        public bool IsParsed { get; set; }


        // TODO: Move to subclass (MergedLog)
        public long Position { get; set; }

        // TODO: Move to subclass (MergedLog)
        public StreamWriter StreamW { get; set; }

        // TODO: Move to subclass (MergedLog)
        public bool IsMerged { get; set; }


        // TODO: Move to subclass (ParsedLog)
        // TODO: Move to subclass (MergedLog)
        public Encoding FileEncoding { get; set; }

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullFilename"></param>
        /// <returns></returns>
        public static LogType GetType(string fullFilename)
        {
            var filename = Path.GetFileName(fullFilename);
            if (filename == null) return LogType.None;

            // TODO: refactor regex

            string type;
            var m = _reAwsLogType.Match(filename);
            if (m.Success)
                type = m.Groups[2].ToString().ToUpper();
            else
            {
                m = _reRackspaceLogType.Match(filename);
                if (!m.Success) return filename.EndsWith(".log") ? LogType.Default : LogType.None;

                type = m.Groups[3].ToString().ToUpper();
            }

            // TODO: Support the following:
            // nginx:
            //  access
            //  sslAccess
            //  error
            //  sslError
            // Velocity
            // Calatina
            // GenerateActivities, (no logs with data found yet!)

            /* Format for Velocity:
                Mon Nov 18 02:05:16 UTC 2013  [error] Left side ($form.v_serviceIdAlias.indexOf("_portfolio")) of '>=' operation has null value at <unknown template>[line 5, column 62]
                Mon Nov 18 02:05:16 UTC 2013  [error] Left side ($form.v_serviceIdAlias.indexOf("_contacts")) of '>=' operation has null value at <unknown template>[line 3, column 93]
                Mon Nov 18 02:05:16 UTC 2013  [error] Left side ($form.v_serviceIdAlias.indexOf("_customer")) of '>=' operation has null value at <unknown template>[line 4, column 61]
             */

            switch (type)
            {
                case "ACCESS": return LogType.None; // LogType.Access;
                case "AUDIT": return LogType.Audit;
                case "BATCH": return LogType.BatchJobs;
                case "BILLING": return LogType.Billing;
                case "CONFIG": return LogType.Config;
                case "D2D": return LogType.D2D;
                case "DBC": return LogType.DbContext;
                case "ENHANCER": return LogType.Enhancers;
                case "EMAIL": return LogType.Email;
                case "ERROR": return LogType.None; // LogType.Error;
                case "EXPORT": return LogType.Export;
                case "GEN": return LogType.None; // LogType.GenerateActivities;
                case "MENU": return LogType.Menu;
                case "QUARTZ": return LogType.Quartz;
                case "REPORT": return LogType.Report;
                case "REST": return LogType.Rest;
                case "SFTPOLLER": return LogType.FilePoller;
                case "SECURE": return LogType.Secure;
                case "SSL":
                    // TODO: Implement detection for ssl.access and ssl.error
                    break;
                case "STRAX": return LogType.ServiceTracker;
                case "SYNC": return LogType.Sync;
                case "TRIGGERS": return LogType.Triggers;
                case "VELOCITY": return LogType.None; // LogType.Velocity;
                case "VELO": return LogType.VeloBean;
                default:
                    return LogType.Default;
            }

            return LogType.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullFilename"></param>
        /// <returns></returns>
        public static string GetProject(string fullFilename)
        {
            var filename = Path.GetFileName(fullFilename);
            if (filename == null) return null;

            // TODO: refactor regex

            var m = _reAwsLogType.Match(filename);
            if (m.Success) return m.Groups[1].ToString();

            m = _reRackspaceLogType.Match(filename);
            if (m.Success || (!m.Success && filename.EndsWith(".log")))
            {
                return m.Groups[2].ToString();
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullFilename"></param>
        /// <returns></returns>
        public static string GetNode(string fullFilename)
        {
            var path = Path.GetDirectoryName(fullFilename);

            return path != null ? path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1) : null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public bool AddEntry(Entry entry)
        {
            IncrementLevels(entry);

            lock (_locker)
            {
                var len = _entries.Length;

                Array.Resize(ref _entries, len + 1);

                _entries[len] = entry;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public bool AddRange(List<Entry> entries)
        {
            lock (_locker)
            {
                if (_entries == null)
                    _entries = new Entry[0];

                var count = entries.Count;
                var len = _entries.Length;

                Array.Resize(ref _entries, len + count);

                for (var i = 0; i < count; i++)
                {
                    var entry = entries[i];
                    IncrementLevels(entry);
                    _entries[len + i] = entry;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SortEntries()
        {
            lock (_locker)
            {
                var len = Entries.Length;
                if (len <= 1) return;

                Array.Sort(_entries, (x, y) => DateTime.Compare(x.Timestamp, y.Timestamp));
                EndTime = len > 0 ? Entries[len - 1].Timestamp : DateTime.MinValue;
                StartTime = len > 0 ? Entries[0].Timestamp : DateTime.MinValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Deserialize()
        {
            return Deserialize(FullFilename + SavedStateFileExt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool Deserialize(string filename)
        {
            var size = FullFilename != null ? new FileInfo(FullFilename).Length : 0;

            using (var fs = File.OpenRead(filename))
            {
                var log = Serializer.Deserialize<Log>(fs);

                // Allow deserialization if Log was instantiated without a filename...
                if (FullFilename != null)
                {
                    // ... but if it was, ensure the '.tree' file is for the same log
                    if (size != log.FileSize || LogType != log.LogType || Project != log.Project)
                        return false;

                    // TODO: In the future, this should probably be done using a hash
                }

                FullFilename = log.FullFilename;
                Filename = log.Filename;
                FileSize = log.FileSize;
                LogType = log.LogType;
                Project = log.Project;
                NodeName = log.NodeName;

                Entries = log.Entries;
                EndTime = log.EndTime;
                StartTime = log.StartTime;

                TraceCount = log.TraceCount;
                DebugCount = log.DebugCount;
                InfoCount = log.InfoCount;
                ErrorCount = log.ErrorCount;
                WarnCount = log.WarnCount;
                FatalCount = log.FatalCount;

                FileEncoding = GetEncoding();
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Serialize()
        {
            return Serialize(FullFilename + SavedStateFileExt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool Serialize(string filename)
        {
            using (var fs = File.Create(filename))
            {
                Serializer.Serialize(fs, this);
            }

            return true;
        }

        ///
        public string ComputeHash()
        {
            using (var md5 = MD5.Create())
                using (var fs = File.OpenRead(FullFilename))
                {
                    return BitConverter.ToString(md5.ComputeHash(fs)).Replace("-", "").ToLower();
                }
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        private void IncrementLevels(Entry entry)
        {
            switch (entry.EntryLevel)
            {
                case EntryLevel.Trace:
                    TraceCount++;
                    break;
                case EntryLevel.Debug:
                    DebugCount++;
                    break;
                case EntryLevel.Info:
                    InfoCount++;
                    break;
                case EntryLevel.Warning:
                    WarnCount++;
                    break;
                case EntryLevel.Error:
                    ErrorCount++;
                    break;
                case EntryLevel.Fatal:
                    FatalCount++;
                    break;
            }
        }

        private Encoding GetEncoding()
        {
            Encoding encoding;

            using (var sr = new StreamReader(FullFilename))
            {
                encoding = sr.CurrentEncoding;
            }

            return encoding;
        }

        #endregion

        #region ICloneable implementation

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return Clone(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public object Clone(bool entries)
        {
            var log = (Log)MemberwiseClone();
            
            log.FullFilename = string.Copy(this.FullFilename);
            log.Filename = string.Copy(this.Filename);
            log.Project = string.Copy(this.Project);
            log.NodeName = string.Copy(this.NodeName);

            log.Entries = new Entry[Entries.Length];
            if (entries)
            {
                for (var i = 0; i < Entries.Length; i++)
                {
                    log.Entries[i] = (Entry) Entries[i].Clone();
                }
            }

            return log;
        }

        #endregion 
    }
}
