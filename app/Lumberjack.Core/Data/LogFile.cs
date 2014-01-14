using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Medidata.Lumberjack.Core.Processing;

namespace Medidata.Lumberjack.Core.Data
{
    public class LogFile : IFieldValueComponent
    {
        #region Nested classes

        /// <summary>
        /// 
        /// </summary>
        public class LogEntryStats
        {
            #region Public properties

            /// <summary>
            /// 
            /// </summary>
            public long Debug { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public long Error { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public long Fatal { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public DateTime FirstEntry { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public long Info { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public DateTime LastEntry { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public long TotalEntries { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public long Trace { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public long Warn { get; set; }

            #endregion

            #region Public methods

            /// <summary>
            /// 
            /// </summary>
            public void Clear() {
                Fatal = 0;
                Trace = 0;
                Error = 0;
                Warn = 0;
                Info = 0;
                Debug = 0;
                TotalEntries = 0;

                FirstEntry = default(DateTime);
                LastEntry = default(DateTime);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="entry"></param>
            public void Add(Entry entry) {

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="entries"></param>
            public void AddRange(Entry[] entries) {

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="entry"></param>
            public void Remove(Entry entry) {

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="entries"></param>
            public void Remove(Entry[] entries) {

            }

            #endregion

            #region ToString override for debugging

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString() {
                return String.Format("{{ " +
                    "Trace = {0}, " +
                    "Debug = {1}, " +
                    "Info = {2}, " +
                    "Warn = {3}, " +
                    "Error = {4}, " +
                    "Fatal = {5}, " +
                    "TotalEntries = {6}, " +
                    "FirstEntry = {7} " +
                    "LastEntry = {8} }}",
                    Trace,
                    Debug,
                    Info,
                    Warn,
                    Error,
                    Fatal,
                    TotalEntries,
                    FirstEntry,
                    LastEntry);
            }

            #endregion
        }

        #endregion

        #region Private fields

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullFilename"></param>
        /// <param name="size"></param>
        public LogFile(string fullFilename, long size) {
            FullFilename = fullFilename;
            Filename = Path.GetFileName(fullFilename);
            Filesize = size;
            EntryStats = new LogEntryStats();
            ProcessTimeElapse = new Dictionary<ProcessTypeEnum, long>()
                {
                    {ProcessTypeEnum.Hash, -1},
                    {ProcessTypeEnum.Filename, -1},
                    {ProcessTypeEnum.Entries, -1},
                };
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public EngineStatusEnum EntryParseStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LogEntryStats EntryStats { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public EngineStatusEnum FilenameParseStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long Filesize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FullFilename { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public EngineStatusEnum HashStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Md5Hash { get; set; }

        public Dictionary<ProcessTypeEnum, long> ProcessTimeElapse { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public SessionFormat SessionFormat { get; set; }

        #endregion

        #region ToString override for debugging

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return String.Format("{{ " +
                "Id = {0}, " +
                "Filename = {1}, " +
                "Filesize = {2}, " +
                "HashStatus = {3}, " +
                "FilenameParseStatus = {4}, " +
                "EntryParseStatus = {5}, " +
                "SessionFormat = {6}, " +
                "Md5Hash = {7}, " +
                "EntryStats = {8}, " +
                "FullFilename = {9} }}",
                Id,
                Filename,
                Filesize,
                HashStatus,
                FilenameParseStatus,
                EntryParseStatus,
                SessionFormat,
                Md5Hash,
                EntryStats,
                FullFilename);
        }

        #endregion
    }
}
