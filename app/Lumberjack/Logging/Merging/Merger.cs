using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Medidata.Lumberjack.Logging.Merging
{
    public class Merger
    {
        #region Private fields

        private static readonly Merger _instance = new Merger();
        private static readonly object _locker = new object();

        private static List<Log> _fileQueue;

        private static volatile bool _isRunning;
        private static volatile bool _isPaused;
        private static volatile bool _isStopping;

        private static Thread _thread;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public static Merger Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPaused
        {
            get { return _isPaused; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsStopping
        {
            get { return _isStopping; }
        }

        /// <summary>
        /// 
        /// </summary>
        public event MergerEventHandler OnStatusChange = null;

        /// <summary>
        /// 
        /// </summary>
        public event MergerEventHandler OnCompleted = null;

        #endregion

        #region Initializers

        public Merger()
        {
            OnStatusChange += Merger_OnStatusChange;
            OnCompleted += Merger_OnCompleted;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="filename"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public bool Start(Session session, string filename, DateTime startTime, DateTime endTime)
        {
            if (_thread != null && _thread.IsAlive)
                return false;

            _isRunning = true;
            _isStopping = false;
            _isPaused = false;

            _fileQueue = session.GetLogs();
            _thread = new Thread(() => WorkerThread(filename, startTime, endTime));
            _thread.Name = "Merger";
            _thread.Start();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            if (!_thread.IsAlive || _isStopping)
                return false;

            _isStopping = true;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Pause()
        {
            if (!_thread.IsAlive || _isPaused)
                return false;

            _isPaused = true;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Resume()
        {
            if (!_thread.IsAlive || !_isPaused)
                return false;

            _isPaused = false;

            return true;
        }

        #endregion

        private static void WorkerThread(string filename, DateTime startTime, DateTime endTime)
        {
            StreamWriter stream = null;
            var success = true;
            long completeEntries = 0;
            long totalEntries = 0;

            try
            {
                // The following assumes that the log entries have been sorted in chronological order already
                GetLogEntriesDateSpan(ref startTime, ref endTime);
                if (Instance.OnStatusChange != null)
                {
                    var message = "Calculated date range of log entries from " + startTime.ToString("dd-MMM-yy HH:mm:ss.fff") + " to " + endTime.ToString("dd-MMM-yy HH:mm:ss.fff") + ".";
                    Instance.OnStatusChange.Invoke(Instance, new MergerEventArgs(MergerAction.CalculatedTimeSpan, message, null));
                }

                int bufferSize;

                GetLogEntriesCount(startTime, endTime, out totalEntries, out completeEntries, out bufferSize);
                if (Instance.OnStatusChange != null)
                {
                    var message = "Eliminated " + completeEntries + " of " + totalEntries + " log entries outside of date range.";
                    Instance.OnStatusChange.Invoke(Instance, new MergerEventArgs(MergerAction.CountedEntries, message, null));
                }

                // TODO: Verifiy that bufferSize is realistic before instantiation the char array

                // Open a StreamReader for each file which we will be reading from
                for (var i = 0; i < _fileQueue.Count; i++)
                {
                    var parsedLog = _fileQueue[i];
                    if (!parsedLog.IsParsed) continue;

                    parsedLog.Stream = new StreamReader(parsedLog.FullFilename, parsedLog.FileEncoding, false, bufferSize);
                    _fileQueue[i] = parsedLog;
                }

                stream = new StreamWriter(filename, false, Encoding.UTF8);
                var lastStatusUpdate = DateTime.Now;
                var buffer = new Char[bufferSize];

                lock (_locker)
                {
                    while (!_isStopping && completeEntries < totalEntries)
                    {
                        while (_isPaused && !_isStopping)
                            Thread.Sleep(1000);

                        Log parsedLog;
                        Entry logEntry;
                        if (!GetNextLogEntry(out parsedLog, out logEntry))
                            break;
                        
                        var count = ReadLogEntry(parsedLog, logEntry, ref buffer);
                        if (count == 0)
                        {
                            break;
                        }
                        
                        success = count == logEntry.Length;
                        if (!success)
                        {
                            // TODO: Provide an error message to the event handle why this failed
                            break;
                        }
                        if (parsedLog.FileEncoding.EncodingName == Encoding.UTF8.EncodingName)
                            stream.Write(buffer, 0, logEntry.Length);
                        else
                        {
                            throw new NotImplementedException();
                            //stream.Write(Encoding.Convert(parsedLog.FileEncoding, Encoding.UTF8, buffer, 0, logEntry.Length));
                        }
                        if (Instance.OnStatusChange != null && DateTime.Now.Subtract(lastStatusUpdate).TotalMilliseconds >= 300)
                        {
                            var message = "Joining log files (" + (((double)completeEntries / (double)totalEntries) * 100).ToString("##0.00") + "% complete)\t " + completeEntries + " of " + totalEntries + " processed.";
                            Instance.OnStatusChange.Invoke(Instance, new MergerEventArgs(MergerAction.Joining, message, null));

                            lastStatusUpdate = DateTime.Now;
                        }

                        completeEntries++;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                success = false;
            }
            finally
            {
                for (var i = 0; i < _fileQueue.Count; i++)
                {
                    var parsedLog = _fileQueue[i];
                    if (parsedLog.Stream == null) continue;

                    parsedLog.Stream.Dispose();
                    parsedLog.Stream = null;

                    _fileQueue[i] = parsedLog;
                }

                if (stream != null)
                    stream.Dispose();
            }

            var action = success ? (_isStopping ? MergerAction.Stopped: MergerAction.Completed) : MergerAction.Failed;
            _isStopping = false;
            _isPaused = false;
            _isRunning = false;

            ResetProcessedEntries();

            if (totalEntries > 0 && Instance.OnStatusChange != null)
            {
                var message = "Joining log files (" + (((double) completeEntries/(double) totalEntries)*100).ToString("##0.00") + "% complete)\t " + completeEntries + " of " + totalEntries + " processed.";
                Instance.OnStatusChange.Invoke(Instance, new MergerEventArgs(MergerAction.Joining, message, null));
            }

            if (Instance.OnCompleted != null)
                Instance.OnCompleted.Invoke(Instance, new MergerEventArgs(action, null, null));
        }

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        private static void ResetProcessedEntries()
        {
            lock (_locker)
            {
                for (var i = 0; i < _fileQueue.Count; i++)
                {
                    var log = _fileQueue[i];
                    
                    if (log.Entries != null)
                    {
                        for (var j = 0; j < log.Entries.Length; j++)
                            log.Entries[j].Processed = false;
                    }

                    log.IsMerged = false;
                    _fileQueue[i] = log;
                    
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private static void GetLogEntriesDateSpan(ref DateTime startTime, ref DateTime endTime)
        {
            var prevTime = DateTime.MaxValue;
            var maxTime = DateTime.MinValue;

            lock (_locker)
            {
                foreach (var parsedLog in _fileQueue.Where(parsedLog => parsedLog.IsParsed))
                {
                    if (parsedLog.StartTime < prevTime)
                        prevTime = parsedLog.StartTime;
                    if (parsedLog.EndTime > maxTime)
                        maxTime = parsedLog.EndTime;
                }
            }

            startTime = startTime > prevTime ? startTime : prevTime;
            endTime = endTime > maxTime ? maxTime : endTime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="totalEntries"></param>
        /// <param name="completeEntries"></param>
        private static void GetLogEntriesCount(DateTime startTime, DateTime endTime , out long totalEntries , out long completeEntries, out int largestEntry)
        {
            totalEntries = 0;
            completeEntries = 0;
            largestEntry = 0;

            for (var i = 0; i < _fileQueue.Count; i++)
            {
                var parsedLog = _fileQueue[i];
                if (!parsedLog.IsParsed) continue;

                totalEntries += parsedLog.Entries.Length;
                if (parsedLog.EndTime < startTime || parsedLog.StartTime > endTime)
                {
                    parsedLog.IsMerged = true;
                    completeEntries += parsedLog.Entries.Length;
                }
                else
                {
                    long processed = 0;
                    var logEntries = parsedLog.Entries.Length;

                    for (var j = 0; j < logEntries; j++)
                    {
                        var entry = parsedLog.Entries[j];

                        if (entry.Timestamp >= startTime && entry.Timestamp <= endTime)
                        {
                            if (entry.Length > largestEntry) largestEntry = entry.Length;

                            continue;
                        }

                        processed++;
                        entry.Processed = true;
                        parsedLog.Entries[j] = entry;
                    }

                    completeEntries += processed;
                    parsedLog.IsMerged = processed == logEntries;
                }

                _fileQueue[i] = parsedLog;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parsedLog"></param>
        /// <param name="logEntry"></param>
        /// <returns></returns>
        private static bool GetNextLogEntry(out Log parsedLog, out Entry logEntry)
        {
            var logIndex = -1;
            var entryIndex = -1;
            var success = true;

            logEntry = new Entry();
            parsedLog = new Log();

            lock (_locker)
            {
                // Find the next log entry
                for (var i = 0; i < _fileQueue.Count; i++)
                {
                    var log = _fileQueue[i];
                    if (log.IsMerged) continue;

                    var logEntries = log.Entries.Length;
                    var j = 0;
                    for (; j < logEntries; j++)
                    {
                        var entry = log.Entries[j];
                        if (entry.Processed) continue;
                        
                        if (entryIndex != -1)
                        {
                            if (entry.Timestamp > logEntry.Timestamp)
                                break;
                        }

                        parsedLog = log;
                        logEntry = entry;
                        entryIndex = j;
                        logIndex = i;

                        break;
                    }

                    if (j < logEntries) continue;

                    log.IsMerged = true;
                    _fileQueue[i] = log;
                }

                if (entryIndex == -1)
                    success = false;
                else
                {
                    // Mark the entry as processed
                    _fileQueue[logIndex].Entries[entryIndex].Processed = true;
                }
            }

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parsedLog"></param>
        /// <param name="logEntry"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        //private static int ReadLogEntry(Log parsedLog, Entry logEntry, ref byte[] buffer)
        private static int ReadLogEntry(Log parsedLog, Entry logEntry, ref Char[] buffer)
        {
            var stream = parsedLog.Stream;
            if (stream == null)
                throw new InvalidOperationException();

           //var pos = stream.Seek(logEntry.Position, SeekOrigin.Begin);

            stream.DiscardBufferedData();
            var pos = stream.BaseStream.Seek(logEntry.Position, SeekOrigin.Begin);

            return stream.Read(buffer, 0, logEntry.Length);
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void Merger_OnCompleted(object source, MergerEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Completed!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void Merger_OnStatusChange(object source, MergerEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(e.Message);
        }

        #endregion
    }
}
