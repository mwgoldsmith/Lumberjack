using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Medidata.Lumberjack.Logging.Parsing
{
    public class Parser
    {   
        #region Private fields

        private static readonly Regex _reNginxEntryA = new Regex(@"(\d{4}/\d\d/\d\d \d\d:\d\d:\d\d) \[([a-z]+)\][^\n]*\n", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex _reNginxEntryE = new Regex(@"[^[]+\[(\d\d/[a-z]{3}/\d{4}:\d\d:\d\d:\d\d) \+0000\][^\n]*\n", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex _reLog4JEntry = new Regex(@"\[{1,2}(?:ENTRY-)?(?:START|!)\]{1,2}\n?(\d\d\-[a-z]{3}\-\d\d \d\d:\d\d:\d\d(?:\.\d\d\d)?) \-?..\-? ([A-Z]+).*?\[{1,2}(?:ENTRY-)?(?:END|!!)\]{1,2}[^\n]*\n", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex _reLog4JEntryV = new Regex(@"[a-z]+ ([a-z]{3} \d\d \d\d:\d\d:\d\d UTC \d\d\d\d)  \[([a-z]+)\][^\n]*\n", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        
        private static readonly Dictionary<LogType, Regex> _reLogEntries = new Dictionary<LogType, Regex>
            {
                { LogType.Access, _reNginxEntryA },
                { LogType.Audit, _reLog4JEntry },
                { LogType.Billing, _reLog4JEntry },
                { LogType.BatchJobs, _reLog4JEntry },
                { LogType.Config, _reLog4JEntry },
                { LogType.D2D, _reLog4JEntry },
                { LogType.DbContext, _reLog4JEntry },
                { LogType.Default, _reLog4JEntry },
                { LogType.Enhancers, _reLog4JEntry },
                { LogType.Email, _reLog4JEntry },
                //{ LogType.Error, _reNginxEntryE }, // Unconfirmed - find a .error.log with data first
                { LogType.Export, _reLog4JEntry },
                { LogType.FilePoller, _reLog4JEntry },
                { LogType.Menu, _reLog4JEntry },
                { LogType.Quartz, _reLog4JEntry },
                { LogType.Report, _reLog4JEntry },
                { LogType.Rest, _reLog4JEntry },
                { LogType.Secure, _reLog4JEntry },
                { LogType.ServiceTracker, _reLog4JEntry },
                { LogType.SslAccess, _reNginxEntryA },
                { LogType.SslError, _reNginxEntryE },
                { LogType.Sync, _reLog4JEntry },
                { LogType.Triggers, _reLog4JEntry },
                { LogType.VeloBean, _reLog4JEntry },
                { LogType.Velocity, _reLog4JEntryV }
            };

        #endregion

        public Parser()
        {
            UseDirAsNodeName = true;
        }

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
            if (!_reLogEntries.ContainsKey(log.LogType))
                return false;

            const int bufferSize = 1024 * 8;

            var buffer = new Char[bufferSize];
            var totalRead = 0L;
            var count = bufferSize;
            var regEx = _reLogEntries[log.LogType];
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
                        count = sr.Read(buffer, 0, (int)(Math.Min(bufferSize, length - totalRead)));
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

                            var entry = new Entry((UInt16)match.Length, time, level, position);
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

                        if (DateTime.Now.Subtract(lastUpdate).TotalSeconds >= 2 && OnStatusChange != null)
                        {
                            len = log.Entries.Length;
                            log.EndTime = len > 0 ? log.Entries[len - 1].Timestamp : DateTime.MinValue;
                            log.StartTime = len > 0 ? log.Entries[0].Timestamp : DateTime.MinValue;

                            OnStatusChange.Invoke(this, new ParserEventArgs(log));
                            lastUpdate = DateTime.Now;
                        }
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

        #endregion
    }
}
