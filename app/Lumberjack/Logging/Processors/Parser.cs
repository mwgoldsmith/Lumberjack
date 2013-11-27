using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Medidata.Lumberjack.Logging.Config;

namespace Medidata.Lumberjack.Logging.Processors
{
    public class Parser : SessionComponentBase
    {   
        #region Private fields

        private static readonly Regex _reNginxEntryA = new Regex(@"(\d{4}/\d\d/\d\d \d\d:\d\d:\d\d) \[([a-z]+)\][^\n]*\n", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex _reNginxEntryE = new Regex(@"[^[]+\[(\d\d/[a-z]{3}/\d{4}:\d\d:\d\d:\d\d) \+0000\][^\n]*\n", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex _reLog4JEntry = new Regex(@"\[{1,2}(?:ENTRY-)?(?:START|!)\]{1,2}\n?(\d\d\-[a-z]{3}\-\d\d \d\d:\d\d:\d\d(?:\.\d\d\d)?) \-?..\-? ([A-Z]+).*?\[{1,2}(?:ENTRY-)?(?:END|!!)\]{1,2}[^\n]*\n", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex _reLog4JEntryV = new Regex(@"[a-z]+ ([a-z]{3} \d\d \d\d:\d\d:\d\d UTC \d\d\d\d)  \[([a-z]+)\][^\n]*\n", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        
        private static readonly Dictionary<string, Regex> _reLogEntries = new Dictionary<string, Regex>
            {
              //  { "Access", _reNginxEntryA },
                { "AUDIT", _reLog4JEntry },
                { "BILLING", _reLog4JEntry },
                { "BATCH_JOBS", _reLog4JEntry },
                { "CONFIG", _reLog4JEntry },
                { "D2D", _reLog4JEntry },
                { "DBC", _reLog4JEntry },
                { "DEFAULT", _reLog4JEntry },
                { "ENHANCERS", _reLog4JEntry },
                { "EMAIL", _reLog4JEntry },
                //{ "Error", _reNginxEntryE }, // Unconfirmed - find a .error.log with data first
                { "EXPORT", _reLog4JEntry },
                { "SFTPOLLER", _reLog4JEntry },
                { "MENU", _reLog4JEntry },
                { "QUARTZ", _reLog4JEntry },
                { "REPORT", _reLog4JEntry },
                { "REST", _reLog4JEntry },
                { "SECURE", _reLog4JEntry },
             //   { "ServiceTracker", _reLog4JEntry },
             //   { "SslAccess", _reNginxEntryA },
             //   { "SslError", _reNginxEntryE },
                { "SYNC", _reLog4JEntry },
                { "TRIGGERS", _reLog4JEntry },
                { "VELO", _reLog4JEntry },
                { "VELOCITY", _reLog4JEntryV }
            };

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public Parser(Session session)
            : base(session)
        {
            UseDirAsNodeName = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool UseDirAsNodeName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public event ParserEventHandler OnStatusChange = null;

        /// <summary>
        /// 
        /// </summary>
        public event ParserEventHandler OnCompleted = null;

        #endregion

        #region Public methods

        public bool Parse(Log log)
        {
            // NOTE: This is horribly inefficient to retrieve this ever time. Cache it or something 
            var parser = Session.FormatConfigurator.Find(log.FormatType, FormatTypeEnum.Entry);

            if (parser == null)
                return false;

            const int bufferSize = 1024*8;

            var buffer = new Char[bufferSize];
            var totalRead = 0L;
            var count = bufferSize;
            var regEx = parser.RegexElement.GetExpression();
            var remaining = "";
            var entries = new List<Entry>(1024);
            int len;
            var lastUpdate = DateTime.MinValue;


            //Stopwatch sw = Stopwatch.StartNew();

            using (var fs = new FileStream(log.FullFilename, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.SequentialScan))
            using (log.Stream = new StreamReader(fs))
            {
                var sr = log.Stream;
                var length = sr.BaseStream.Length;

                if (length == 0)
                {
                    log.IsParsed = true;
                    return true;
                }

                log.FileEncoding = sr.CurrentEncoding;
                log.BytesParsed = 0;
                log.IsParsed = false;

                while (length > 0 && count > 0)
                {
                    count = sr.Read(buffer, 0, (int) (Math.Min(bufferSize, length - totalRead)));
                    if (count == 0) break;

                    var text = new string(buffer, 0, count);
                    var matches = regEx.Matches(remaining + text);
                    var lastPos = 0;

                    foreach (Match match in matches)
                    {
                        if (!match.Success) continue;

                        var time = match.Groups[1].Value;
                        var level = match.Groups[2].Value;
                        var position = log.BytesParsed + log.FileEncoding.GetByteCount((remaining + text).Substring(0, match.Index));

                        var entry = new Entry((UInt16) match.Length, time, level, position);
                        entries.Add(entry);

                        lastPos = match.Index + match.Length - remaining.Length;
                    }

                    log.BytesParsed += log.FileEncoding.GetByteCount(text);
                    log.BytesParsed += log.FileEncoding.GetByteCount(remaining);
                    log.BytesParsed -= count - lastPos;

                    totalRead += count;
                    if (lastPos < count)
                        remaining = text.Substring(lastPos);

                    log.AddRange(entries);
                    entries.Clear();

                    if (!(DateTime.Now.Subtract(lastUpdate).TotalSeconds >= 2) || OnStatusChange == null) continue;
                    
                    len = log.Entries.Length;
                    log.EndTime = len > 0 ? log.Entries[len - 1].Timestamp : DateTime.MinValue;
                    log.StartTime = len > 0 ? log.Entries[0].Timestamp : DateTime.MinValue;

                    OnStatusChange.Invoke(this, new ParserEventArgs(log));
                    lastUpdate = DateTime.Now;
                }
            }
            //sw.Stop();
            //System.Diagnostics.Debug.WriteLine("Elapsed: " + sw.ElapsedMilliseconds + "; entries: " + log.Entries.Length);
            //Elapsed: 10399; entries: 12765; bytes: 5166983            1024
            //Elapsed: 19166; entries: 12765; bytes: 5166983            512
            //Elapsed: 6021; entries: 12765         256
            //Elapsed: 1932; entries: 12765         64
            //Elapsed: 869; entries: 12765          8
            //Elapsed: 907; entries: 12765          8
            log.SortEntries();
            log.IsParsed = true;

            if (OnStatusChange != null)
            {
                OnCompleted.Invoke(this, new ParserEventArgs(log));
            }

            return true;
        }


        public bool ParseFilename(Log log)
        {
            // NOTE: This is horribly inefficient to retrieve this ever time. Cache it or something 
            var parsers = Session.FormatConfigurator.FindAll(FormatTypeEnum.Filename);
            var success = true;

            foreach (var p in parsers)
            {
                if (!success) break;

                var regex = p.Value.RegexElement.GetExpression();

                var m = regex.Match(log.Filename);
                if (!m.Success) continue;

                log.FormatType = p.Key;

                // Extract any fields defined within parser
                foreach (var f in p.Value.Fields)
                {
                    string value = null;
                    var groups = f.GetGroups();

                    // Take the first capture which was successful
                    for (var i = 0; i < groups.Length; i++)
                    {
                        if (!m.Groups[groups[i]].Success) continue;

                        value = m.Groups[groups[i]].ToString();
                        break;
                    }

                    // Check if field was not found but has default value
                    if (value == null && f.Default != null)
                        value = f.Default;

                    // Check if required field could not be found and no default value
                    if (f.Required && value == null)
                    {
                        // TODO: Notify that the filename format is invalid and a required field is missing

                        success = false;
                        break;
                    }

                    switch (f.Identifier)
                    {
                        case "PROJECT":
                            log.Project = value;
                            break;
                        case "STAGE":
                            log.Stage = value;
                            break;
                    }
                }
            }

            return success;
        }

        #endregion
    }
}
