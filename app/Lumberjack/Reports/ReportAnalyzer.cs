using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medidata.Lumberjack.Reports
{
    class ReportAnalyzer
    {
        /*
         * using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using Medidata.CTMS.LogAnalyer.UI;

namespace Medidata.CTMS.LogAnalyer
{
    public class LogFile
    {
        public enum LogType { DBC, REPORT, OTHER }

        public string Filename { get; set; }
        public string FullPath { get; set; }
        public long Size { get; set; }
        public LogType Type { get; set; }
        public string Client { get; set; }
        public string Timestamp { get; set; }
    }

    public class LogPair
    {
        public List<LogFile> DbcLogs { get; set; }
        public List<LogFile> ReportLogs { get; set; }

        public LogPair()
        {
            DbcLogs = new List<LogFile>();
            ReportLogs = new List<LogFile>();
        }
    }

    public class LogOutput
    {
        private string _timestamp;

        public string Client { get; set; }
        public string User { get; set; }
        public string Service { get; set; }
        public string Other { get; set; }
        public string ReportRef { get; set; }
        public string ReportId { get; set; }
        public string RegexTime { get; private set; }
        public string Timestamp {
            get { return _timestamp; }
            set
            {
               _timestamp = value;
                RegexTime = GetTimeRegex(_timestamp);
            }
        }

        public LogOutput() 
        {
            Client = "";
            Timestamp = "";
            Service = "";
            User = "";
            Other = "";
            ReportRef = "";
            ReportId = "";
            RegexTime = "";
        }

        private string GetTimeRegex(string timestamp)
        {
            var sb = new StringBuilder();

            if (timestamp.Length == "dd-mmm-yy nn:nn:nn.nnn".Length)
            {
                var lastchar = timestamp.Substring("dd-mmm-yy nn:nn:n".Length, 1);

                sb.Append(timestamp.Substring(0, "dd-mmm-yy".Length).Replace("-", @"\-"));
                sb.Append(@"\s");
                sb.Append(timestamp.Substring("dd-mmm-yy ".Length, "nn:nn:n".Length));

                int sub;
                if (!int.TryParse(timestamp.Substring("dd-mmm-yy nn:nn:nn.".Length, 3), out sub))
                    sb.Append(lastchar);
                else
                {
                    int lastnum;
                    if (!int.TryParse(lastchar, out lastnum))
                        sb.Append(lastchar);
                    else
                    {
                        if (sub > 800)
                            lastchar = (lastnum + 1).ToString();
                        else if (sub < 200)
                            lastchar = (lastnum - 1).ToString();

                        sb.AppendFormat("[{0}|{1}]", lastchar, lastnum);
                    }
                }
            }

            return sb.ToString();
        }

        public string ToCsvString()
        {
            var csv = String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"",
                        Client, Timestamp, User, Service,
                        Other, ReportRef, ReportId);

            return csv;
        }
    }

    public class FileParser
    {
        private const int ChunkSize = (1024 * 1024 * 40); // 40 Mb

        #region Private fields

        private static readonly char[] _buffer = new char[Math.Min(ChunkSize, 1024 * 1024)];
        private string _path;

        private readonly Dictionary<string, LogPair> _logFiles;
        private readonly Dictionary<string, LogOutput> _logOutput;
        private readonly Status _currentStatus;
        private readonly Status _totalStatus;
        private readonly TextQueue _statusPanel;

        #endregion

        #region Properties

        public string LogPath
        { 
            get { return _path; }
            set
            {
                _path = value;
                FindLogFiles(_path);
            }
        }

        public Dictionary<string, LogPair> LogFiles
        {
            get { return _logFiles; }
        }

        #endregion

        #region Initializers

        public FileParser()
        {
            DisplayStatus control = new TotalStatus(9, 2, 60, 7, "totalStatus");
            Program.Ui.AddControl(control);
            _totalStatus = new Status(control);

            control = new CurrentStatus(19, 2, 60, 7, "currentStatus");
            Program.Ui.AddControl(control);
            _currentStatus = new Status(control);

            _statusPanel = new TextQueue(4, 67, "txtParseErrors");
            _statusPanel.Width = Console.WindowWidth - _statusPanel.Left - 4;
            _statusPanel.Height = 23;
            _statusPanel.Transparent = false;
            _statusPanel.BgColor = ConsoleColor.Blue;
            _statusPanel.FgColor = ConsoleColor.White;
            Program.Ui.AddControl(_statusPanel);

            _logFiles = new Dictionary<string, LogPair>(StringComparer.Ordinal);
            _logOutput = new Dictionary<string, LogOutput>(StringComparer.Ordinal);
        }

        #endregion

        public int FindLogFiles(string path)
        {
            //var status = Program.Ui["txtStatus"] as TextQueue;
            var fileList = Directory.GetFiles(path);

            _totalStatus.Reset();
            _logFiles.Clear();
            _path = path;

            foreach (string file in fileList)
            {
                var filename = Path.GetFileName(file);
                if (filename == null) continue;

                var m = Regex.Match(filename, @"^([^A-Z]+)_([A-Z]+)\-([\d]{8}).*$", RegexOptions.CultureInvariant);
                if (m.Groups.Count < 4) continue;

                var key = m.Groups[1].Value + '-' + m.Groups[3].Value;
                var logPair = new LogPair();
                if (_logFiles.ContainsKey(key))
                    logPair = _logFiles[key];

                var log = new LogFile();
                if (m.Groups[2].Value == "DBC") log.Type = LogFile.LogType.DBC;
                else if (m.Groups[2].Value == "REPORT") log.Type = LogFile.LogType.REPORT;
                else continue;

                log.Client = m.Groups[1].Value;
                log.Filename = filename;
                log.Timestamp = m.Groups[3].Value;
                log.FullPath = LogPath + '\\' + filename;

                var f = new FileInfo(log.FullPath);

                log.Size = f.Length;
                if (log.Type == LogFile.LogType.DBC)
                {
                    _totalStatus.TotalDbc++;
                    logPair.DbcLogs.Add(log);
                }
                else if (log.Type == LogFile.LogType.REPORT)
                {
                    _totalStatus.TotalReport++;
                    logPair.ReportLogs.Add(log);
                }

                _totalStatus.TotalBytes += log.Size;
                _logFiles[key] = logPair;
            }

            return _logFiles.Count;
        }

        public void ParseLogs()
        {
            _currentStatus.Reset();
            _totalStatus.StartTime = DateTime.Now;

            foreach (LogPair logPair in _logFiles.Values)
            {
                var hasReports = false;

                foreach (LogFile log in logPair.ReportLogs)
                {
                    if (Program.State == Program.AppState.Exiting) break;
                    if (log.Size > 0)
                    {
                        hasReports = true;
                        ParseReportLog(log);
                    }
                    else
                    {
                        AppendToLog("File \"" + log.Filename + "\" contains no data; skipping.", true);
                        _totalStatus.TotalReport--;
                    }
                }

                foreach (LogFile log in logPair.DbcLogs)
                {
                    if (Program.State == Program.AppState.Exiting) break;
                    if (hasReports && logPair.ReportLogs.Count > 0) ParseDbcLog(log);
                    else if (log.Size == 0)
                    {
                        AppendToLog("File \"" + log.Filename + "\" contains no data; skipping.", true);
                        _totalStatus.TotalDbc--;
                    }
                    else
                    {
                        AppendToLog("File \"" + log.Filename + "\" has no corresponding REPORT log; skipping.", true);
                        _totalStatus.TotalBytes -= log.Size;
                        _totalStatus.TotalDbc--;
                    }
                }

                FlushOutput();
                if (Program.State == Program.AppState.Exiting) break;
            }

            if (Program.State != Program.AppState.Exiting)
                Program.State = Program.AppState.Done;
        }

        public void ParseReportLog(LogFile log)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            var regEx = new Regex(@"\[\[START\]\](\d\d\-[a-zA-Z]{3}\-\d\d\s\d\d:\d\d:\d\d\.\d\d\d).*?([a-z_]+)\s\-\s([^:]*?):[^:]*:\s+[^:]*:\s(.*)\s+TMPL:.*", RegexOptions.CultureInvariant);
            int start = 0;

            _currentStatus.Reset();
            _currentStatus.TotalBytes = log.Size;
            _currentStatus.Filename = log.Filename;
            _totalStatus.CurReport++;

            while (true)
            {
                if (Program.State == Program.AppState.Exiting)
                    break;
                while (Program.State == Program.AppState.Paused)
                    Thread.Yield();

                string fileChunk = ReadFileChunk(log.FullPath, start, ChunkSize);
                start += fileChunk.Length;
                if (fileChunk.Length == 0) break;

                _totalStatus.RefreshElapsedTime();
                _totalStatus.RefreshDataSpeed(TimeSpan.Zero, fileChunk.Length);
                _totalStatus.CurrentBytes += fileChunk.Length;
                _currentStatus.CurrentBytes += fileChunk.Length;

                stopwatch.Reset();
                stopwatch.Start();

                var matches = regEx.Matches(fileChunk);
                foreach (Match m in matches)
                {
                    if (Program.State == Program.AppState.Exiting)
                        break;
                    while (Program.State == Program.AppState.Paused)
                        Thread.Yield();

                    var output = new LogOutput
                    {
                        Client = log.Client,
                        Timestamp = m.Groups[1].Value, 
                        Service = m.Groups[2].Value, 
                        Other = m.Groups[3].Value,
                        ReportRef = m.Groups[4].Value
                    };

                    var key = log.Client + log.Timestamp + output.Timestamp;
                    if (_logOutput.ContainsKey(key))
                    {
                        _totalStatus.DupRef++;
                        _currentStatus.DupRef++;
                    }
                    else
                    {
                        _totalStatus.NumRef++;
                        _currentStatus.NumRef++;
                        _logOutput.Add(key, output);
                    }

                    _totalStatus.RefreshElapsedTime();
                }

                stopwatch.Stop();
                _totalStatus.RefreshDataSpeed(stopwatch.Elapsed, 0);
            }
        }

        public void ParseDbcLog(LogFile log)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            int start = 0;

            _currentStatus.Reset();
            _currentStatus.TotalBytes = log.Size;
            _currentStatus.Filename = log.Filename;
            _totalStatus.CurDbc++;

            while (true)
            {
                if (Program.State == Program.AppState.Exiting)
                    break;
                while (Program.State == Program.AppState.Paused) 
                    Thread.Yield();

                var fileChunk = ReadFileChunk(log.FullPath, start, Math.Min(ChunkSize, log.Size));
                start += fileChunk.Length;
                if (fileChunk.Length == 0) break;

                _totalStatus.RefreshDataSpeed(TimeSpan.Zero, fileChunk.Length);

                _currentStatus.CurrentBytes += fileChunk.Length;
                _totalStatus.CurrentBytes += fileChunk.Length;
                _totalStatus.RefreshElapsedTime();

                var keys = new string[_logOutput.Keys.Count];
                _logOutput.Keys.CopyTo(keys, 0);
                for (var i = 0; i < keys.Length; i++)
                {
                    if (Program.State == Program.AppState.Exiting) break;
                    while (Program.State == Program.AppState.Paused) Thread.Yield();
                    _totalStatus.RefreshElapsedTime();

                    stopwatch.Reset();
                    stopwatch.Start();

                    var output = _logOutput[keys[i]];
                    if (output.Client != log.Client) continue;

                    if (output.User.Length == 0)
                    {
                        var regStr = @"\[\[START\]\]" + output.RegexTime + @"[^[]*?select\slogged_out[^=]*?='([^']*)'[^[]*\[\[END\]\]";
                        var match = Regex.Match(fileChunk, regStr, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
                        if (match.Success)
                        {
                            output.User = match.Groups[1].Value;
                            _currentStatus.NumUsers++;
                            _totalStatus.NumUsers++;
                        }
                        else
                        {
                            _statusPanel.AddLine("Entry " + output.Timestamp + ": no user found");
                        }
                    }

                    if (output.ReportId.Length == 0)
                    {
                        var regStr = @"\[\[START\]\]" + output.RegexTime + @"[^[]*?:[^[]*executeQuery:[^[]*report_dfn_rec[^[]*id(?:entifier)?\s?=\s?([\d]+|'[A-Z\d]+')?[^[]*\[\[END\]\]";
                        var match = Regex.Match(fileChunk, regStr, RegexOptions.CultureInvariant);
                        if (match.Success)
                        {
                            output.ReportId = match.Groups[1].Value;
                            _currentStatus.NumIDs++;
                            _totalStatus.NumIDs++;
                        }
                        else
                        {
                            _statusPanel.AddLine("Entry " + output.Timestamp + ": no report ID found");
                        }
                    }

                    var key = log.Client + log.Timestamp + output.Timestamp;
                    _logOutput[key] = output;

                    stopwatch.Stop();
                    _totalStatus.RefreshDataSpeed(stopwatch.Elapsed, 0);
                    if (i % 4 == 0) _statusPanel.Refresh();
                }

                _statusPanel.Refresh();
            }
        }

        public void FlushOutput()
        {
            var sb = new StringBuilder();

            foreach (LogOutput log in _logOutput.Values)
                sb.AppendLine(log.ToCsvString());

            File.AppendAllText(_path + @"\output.csv", sb.ToString());

            _logOutput.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="refresh"></param>
        public void AppendToLog(string line, bool refresh)
        {
            var status = Program.Ui["txtStatus"] as TextQueue;
            if (status == null)
                throw new InvalidOperationException();

            status.AddLine(line, refresh);
            File.AppendAllText(_path + @"\ctms_analysis.csv", line + "\r\n");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="start"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public string ReadFileChunk(string filename, long start, long chunkSize)
        {
            var sb = new StringBuilder((int)chunkSize);
            var numRead = 0L;

            try
            {
                using (var sr = new StreamReader(filename))
                {
                    sr.BaseStream.Seek(start, SeekOrigin.Begin);
                    while (!sr.EndOfStream)
                    {
                        if (numRead < chunkSize)
                        {
                            var bytes = sr.ReadBlock(_buffer, 0, _buffer.Length);
                            var line = new char[bytes];
                            Buffer.BlockCopy(_buffer, 0, line, 0, sizeof(char)*bytes);

                            sb.Append(line);
                            numRead += line.Length;
                        }
                        else
                        {
                            var bytes = sr.ReadBlock(_buffer, 0, Math.Min(_buffer.Length, 1024));
                            var line = new char[bytes];
                            Buffer.BlockCopy(_buffer, 0, line, 0, bytes);

                            var strLine = new String(line);
                            var pos = strLine.LastIndexOf("[[END]]");
                            if (pos > 0) strLine = strLine.Substring(0, pos);

                            sb.Append(strLine);
                            numRead += strLine.Length;
                            
                            if (pos > 0) break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var status = Program.Ui["txtStatus"] as TextQueue;
                if (status == null)
                    throw new InvalidOperationException();

                status.AddLine(e.Message, true);
            }

            return sb.ToString();
        }
    }
}*/
    }
}
