using System.Diagnostics;
using System.IO;
using System.Threading;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Data.Fields.Values;

namespace Medidata.Lumberjack.Core.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class LogFileCollection : CollectionBase<IFieldValueComponent>, IValueItemCollection<LogFile>
    {
        #region Private fields

       // private static long _id;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public LogFileCollection(UserSession session)
            : base(session) {
        }

        #endregion

        #region Indexers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public LogFile this[int index] {
            [DebuggerStepThrough]
            get { return _items[index] as LogFile; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="filename"/> is null. </exception>
        /// <exception cref="T:System.ArgumentException">The file name is empty, contains only white spaces, or contains invalid characters. </exception>
        /// <exception cref="T:System.NotSupportedException"><paramref name="filename"/> contains a colon (:) in the middle of the string. </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">Access to <paramref name="filename"/> is denied. </exception>
        /// <exception cref="T:System.IO.IOException"><see cref="M:System.IO.FileSystemInfo.Refresh"/> cannot update the state of the file or directory. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file does not exist.-or- The Length property is called for a directory. </exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. </exception>
        public void Add(string filename) {
            var size = (new FileInfo(filename)).Length;

            Add(new LogFile(filename, size));//{ Id = GetNextId() });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filenames"></param>
        /// <exception cref="T:System.ArgumentNullException">A filename in <paramref name="filenames"/> is null. </exception>
        /// <exception cref="T:System.ArgumentException">The file name is empty, contains only white spaces, or contains invalid characters. </exception>
        /// <exception cref="T:System.NotSupportedException">A filename in <paramref name="filenames"/> contains a colon (:) in the middle of the string. </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">Access to a file in <paramref name="filenames"/> is denied. </exception>
        /// <exception cref="T:System.IO.IOException"><see cref="M:System.IO.FileSystemInfo.Refresh"/> cannot update the state of the file or directory. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file does not exist.-or- The Length property is called for a directory. </exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. </exception>
        public void Add(string[] filenames) {
            var len = filenames.Length;
            var logFiles = new LogFile[len];

            // Create array of new LogFile instances to add to the session
            for (var i = 0; i < len; i++) {
                var filename = filenames[i];
                var size = (new FileInfo(filename)).Length;

                logFiles[i] = new LogFile(filename, size);// { Id = GetNextId() };
            }

            Add(logFiles);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //[DebuggerStepThrough]
        //public static int GetNextId() {
        //    return (int)Interlocked.Increment(ref _id);
        //}

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
            
            OnItemUpdated(logFiles);
        }

        #endregion
    }
}
