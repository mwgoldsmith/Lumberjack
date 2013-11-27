using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Medidata.Lumberjack.Logging.Config.Fields;
using Medidata.Lumberjack.Logging.Config.Formats;
using Medidata.Lumberjack.Logging.Config.Nodes;
using Medidata.Lumberjack.Logging.Processors;

namespace Medidata.Lumberjack.Logging
{
    public class Session
    {
        #region Private fields
        
        private volatile bool _dirty = false;

        private readonly List<Log> _logs = new List<Log>();
        private long _logCount = 0;
        private long _totalBytes = 0;

        private static readonly object _locker = new object();

        #endregion

        #region Initializers
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public Session(string name)
        {
            Name = name;
            
            Parser = new Parser(this);

            NodeConfigurator = new NodeConfigurator();
            FormatConfigurator = new FormatConfigurator();
            FieldConfigurator = new FieldConfigurator();
        }

        /// <summary>
        /// 
        /// </summary>
        public Session()
            : this("")
        {
            
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string LogDirectory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDirty
        {
            get { return _dirty; }
            set { _dirty = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int TotalLogs
        {
            get { return (int)Interlocked.Read(ref _logCount); }
        }

        /// <summary>
        /// 
        /// </summary>
        public long TotalBytes
        {
            get { return (int)Interlocked.Read(ref _totalBytes); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Parser Parser { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public NodeConfigurator NodeConfigurator { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public FormatConfigurator FormatConfigurator { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public FieldConfigurator FieldConfigurator { get; private set; }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool Load(string filename)
        {
            Filename = filename;

            // TODO: Implement

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            // TODO: Implement

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string FindRootLogDirectory()
        {
            // TODO: Implement

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool IsLogParseCached(Log log)
        {
            var cached = false;

                cached = File.Exists(log.FullFilename + Log.SavedStateFileExt);
            
            return cached;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool CacheLogParse(Log log)
        {
            return log.Serialize(log.FullFilename + Log.SavedStateFileExt);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearLogs()
        {
            lock (_locker)
            {
                _logs.Clear();

                _logCount = 0;
                _dirty = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public Log AddLog(string filename)
        {
            var log = new Log(filename);

            // Obtain whataver fields we can from the filename
            Parser.ParseFilename(log);

            return AddLog(log) ? log : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool AddLog(Log log)
        {
            lock (_locker)
                if (_logs.All(l => l.FullFilename != log.FullFilename))
                {
                    _logCount++;

                    _logs.Add(log);
                    _totalBytes += log.FileSize;

                    _dirty = true;
                }
            
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filenames"></param>
        /// <returns></returns>
        public Log[] AddLogRange(ICollection<string> filenames)
        {
            var logs = filenames.Select(f =>
                {
                    var log = new Log(f);

                    // Obtain whataver fields we can from the filename
                    Parser.ParseFilename(log);

                    System.Diagnostics.Debug.WriteLine("{0}\t\t{1}\t\t{2}", log.FormatType, log.Project, log.Filename);
                    return log;
                }).ToList();

            return AddLogRange(logs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logs"></param>
        /// <returns></returns>
        public Log[] AddLogRange(ICollection<Log> logs)
        {
            var added = new List<Log>();

            lock (_locker)
            {
                foreach (var log in logs)
                {
                    // Verify a log with the same filename doesn't already exist
                    var exists = _logs.Any(l => l.FullFilename == log.FullFilename);
                    if (exists) continue;

                    _logCount++;
                    _totalBytes += log.FileSize;

                    added.Add(log);
                    _logs.Add(log);
                }

                _dirty = true;
            }

            return added.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>If the log was removed, true; otherwise, false.</returns>
        public bool RemoveLog(string filename)
        {
            var removed = false;

            lock (_locker)
            {
                // Determine logs to remove
                var log = _logs.Find(l => l.FullFilename.ToUpper() == filename.ToUpper());
                if (log != null)
                {
                    _logs.Remove(log);
                    _logCount--;
                    _totalBytes -= log.FileSize;
                    _dirty = true;

                    removed = true;
                }
            }

            return removed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        /// <returns>If the log was removed, true; otherwise, false.</returns>
        public bool RemoveLog(Log log)
        {
            return RemoveLog(log.FullFilename);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filenames"></param>
        /// <returns></returns>
        public Log[] RemoveLogRange(ICollection<string> filenames)
        {
            var removed = new List<Log>();
            
            lock (_locker)
            {
                foreach (var f in filenames)
                {
                    // Determine logs to remove
                    var log = _logs.Find(l => l.FullFilename.ToUpper() == f.ToUpper());
                    if (log == null) continue;

                    _logs.Remove(log);
                    _logCount--;
                    _totalBytes -= log.FileSize;

                    removed.Add(log);
                }

                // Only set dirty flag if logs were removed
                _dirty |= removed.Count > 0;
            }

            return removed.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logs"></param>
        /// <returns></returns>
        public Log[] RemoveLogRange(ICollection<Log> logs)
        {
            var removed = logs.Select(l => l.FullFilename).ToList();

            return RemoveLogRange(removed);
        }

        /// <summary>
        /// Retreives a list of the logs within the session. NOTE: The return value is a shallow
        /// clone and not a reference to the actual list used internally. As such, modifying the
        /// list will not affect the logs within the session. However, the Log items reference
        /// the same object and CAUTION should be used when accessing from multiple threads.
        /// </summary>
        /// <returns></returns>
        public List<Log> GetLogs()
        {
            var logs = new List<Log>();

            lock (_locker)
                logs.AddRange(_logs);

            return logs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public Log FindLog(string filename)
        {
            Log log;

            lock (_locker)
                log = _logs.Find(l => l.FullFilename.ToUpper() == filename.ToUpper());
            
            return log;
        }

        #endregion

    }
}
