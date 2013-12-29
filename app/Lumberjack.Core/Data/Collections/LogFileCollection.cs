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
        private int _id;

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
        /// <param name="log"></param>
        public override void Add(LogFile log) {
            base.Add(log);

            SessionInstance.ProcessController.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFiles"></param>
        public override void Add(LogFile[] logFiles) {
            base.Add(logFiles);

            SessionInstance.ProcessController.Start();
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
        /// <param name="items"></param>
        public void Remove(string[] items) {

        }

        #endregion
    }
}
