using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;

namespace Medidata.Lumberjack.Core.Data.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class LogFileCollection : CollectionBase<LogFile>
    {
        #region Private fields

        private int _id;
        
        #endregion

        #region Event Handlers

        /// <summary>
        /// 
        /// </summary>
        public event LogFileUpdatedHandler LogFileRemoved;
        /// <summary>
        /// 
        /// </summary>
        public event LogFileUpdatedHandler LogFileAdded;
        /// <summary>
        /// 
        /// </summary>
        public event LogFileUpdatedHandler LogFileUpdated;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public LogFileCollection(UserSession session) : base(session) {
        }

        #endregion

        #region Indexers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override LogFile this[int index] {
            get { return _items[index]; }
        }

        #endregion

        #region Method overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        public override void Add(LogFile logFile) {
            base.Add(logFile);
            OnLogFileAdded(new[]{logFile});

            ////
            SessionInstance.ProcessController.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFiles"></param>
        public override void Add(LogFile[] logFiles) {
            base.Add(logFiles);
            OnLogFileAdded(logFiles);

            ////
            SessionInstance.ProcessController.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="logFile"></param>
        public override void Insert(int index, LogFile logFile) {
            base.Insert(index, logFile);
            OnLogFileAdded(new[] { logFile });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        /// <returns></returns>
        public override bool Remove(LogFile logFile) {
            var ret = base.Remove(logFile);

            OnLogFileRemoved(new[] { logFile });

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public override void RemoveAt(int index) {
            var logFile = _items[index];

            base.RemoveAt(index);

            OnLogFileRemoved(new[] { logFile });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFiles"></param>
        public override void Remove(LogFile[] logFiles) {
            base.Remove(logFiles);
            OnLogFileRemoved(logFiles);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public void Add(string filename) {
            var size = (new FileInfo(filename)).Length;

            Add(new LogFile(filename, size) { Id = _id++ });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filenames"></param>
        public void Add(string[] filenames) {
            var logFiles = new LogFile[filenames.Length];

            // Create array of new LogFile instances to add to the session
            for (var i = 0; i < filenames.Length; i++) {
                var filename = filenames[i];
                var size = (new FileInfo(filename)).Length;

                logFiles[i] = new LogFile(filename, size) { Id = _id++ };
            }

            Add(logFiles);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public LogFile Find(string filename) {
            LogFile log = null;

            lock (_locker) {
                for (var i = 0; i < _items.Count && log == null; i++) {
                    if (_items[i].FullFilename.Equals(filename)) {
                        log = _items[i];
                    }
                }
            }

            return log;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFile"></param>
        public void Update(LogFile logFile) {
            Update(new[] {logFile});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFiles"></param>
        public void Update(LogFile[] logFiles) {
            // TODO: When LogFile objects are made immutable outside of this collection, 
            // TODO: this will be the only method to actually update the LogFile's data
            
            OnLogFileUpdated(logFiles);
        }

        #endregion
       
        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFiles"></param>
        private void OnLogFileRemoved(LogFile[] logFiles) {
            if (LogFileRemoved != null) {
                LogFileRemoved(this, new LogFilesUpdatedEventArgs(logFiles));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFiles"></param>
        private void OnLogFileAdded(LogFile[] logFiles) {
            if (LogFileAdded != null) {
                LogFileAdded(this, new LogFilesUpdatedEventArgs(logFiles));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFiles"></param>
        private void OnLogFileUpdated(LogFile[] logFiles) {
            if (LogFileUpdated != null) {
                LogFileUpdated(this, new LogFilesUpdatedEventArgs(logFiles));
            }
        }

        #endregion
    }
}
