using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Data.Collections;

namespace Medidata.Lumberjack.UI
{
    public partial class LogPropertiesForm : Form
    {
        #region Nested structs

        /// <summary>
        /// 
        /// </summary>
        protected struct AggregateProperty
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="count"></param>
            public AggregateProperty(string name, int count) : this() {
                Name = name;
                Count = count;
            }

            /// <summary>
            /// 
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int Count { get; set; }
        }

        #endregion

        #region Private fields

        private readonly object[] _singleProperties = new object[]
            {
                "Id",
                "Full Filename",
                "Filesize",
                "MD5 Hash",
                "Session Format",
                "Hashing Status",
                "Filename Parsing Status",
                "Entry Parsing Status",
                "Entry Statistics"
            };

        private readonly object[] _multiProperties = new object[]
            {
                "Filesize",
                "Session Format",
                "Hashing Status",
                "Filename Parsing Status",
                "Entry Parsing Status",
                "Entry Statistics"
            };

        private LogFile[] _logFiles;

        #endregion

        #region Initializers

        /// <summary>
        /// Creates a new LogPropertiesForm instance
        /// </summary>
        public LogPropertiesForm(LogFile[] logFiles) {
            InitializeComponent();
            _logFiles = logFiles;
        }

        #endregion

        #region Form event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogPropertiesForm_Load(object sender, EventArgs e) {
            if (_logFiles.Length == 1) {
                logFileTextBox.Text = _logFiles[0].Filename;
                logPropertyListBox.Items.AddRange(_singleProperties);
            } else {
                logFileTextBox.Text = _logFiles.Length + " file(s)";
                logPropertyListBox.Items.AddRange(_multiProperties);
            }

            logPropertyListBox.SelectedIndices.Add(0);
        }

        #endregion

        #region Control event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e) {
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logPropertyListBox_SelectedIndexChanged(object sender, EventArgs e) {
            var selected = logPropertyListBox.SelectedItem.ToString();
            var text = "";

            switch (selected) {
                case "Id":
                    if (_logFiles.Length == 1)
                        text = _logFiles[0].Id.ToString();

                    break;
                case "Filesize":
                    text = _logFiles.Sum(l => l.Filesize) + " byte(s)";
                    break;
                case "Full Filename":
                    if (_logFiles.Length == 1)
                        text = _logFiles[0].FullFilename;

                    break;
                case "MD5 Hash":
                    if (_logFiles.Length == 1)
                        text = _logFiles[0].Md5Hash;

                    break;
                case "Session Format":
                    if (_logFiles.Length == 1) {
                        var sessionFormat = _logFiles[0].SessionFormat;
                        text = String.Format(
                            "Id:               {0}" + Environment.NewLine +
                            "Name:             {1}" + Environment.NewLine +
                            "Reference:        {2}" + Environment.NewLine +
                            "Timestamp Format: {3}" + Environment.NewLine + 
                            "Contexts:" + Environment.NewLine,
                            sessionFormat.Id,
                            sessionFormat.Name,
                            sessionFormat.Reference,
                            sessionFormat.TimestampFormat);

                        var contextText = GetContextText(sessionFormat.Contexts, FormatContextEnum.Filename, 2);
                        if (contextText != null)
                            text += contextText;
                        contextText = GetContextText(sessionFormat.Contexts, FormatContextEnum.Entry, 2);
                        if (contextText != null)
                            text += contextText;
                        contextText = GetContextText(sessionFormat.Contexts, FormatContextEnum.Content, 2);
                        if (contextText != null)
                            text += contextText;
                    } else {
                        text = GetAggregateProperty(v => v.SessionFormat, n => n.Name);
                    }

                    break;
                case "Hashing Status":
                    text = GetAggregateProperty(v => v.HashStatus, n => n.ToString());
                    break;
                case "Filename Parsing Status":
                    text = GetAggregateProperty(v => v.FilenameParseStatus, n => n.ToString());
                    break;
                case "Entry Parsing Status":
                    text = GetAggregateProperty(v => v.EntryParseStatus, n => n.ToString());
                    break;
                case "Entry Statistics":
                    text = String.Format(
                        "TRACE:         {0}" + Environment.NewLine +
                        "DEBUG:         {1}" + Environment.NewLine +
                        "INFO:          {2}" + Environment.NewLine +
                        "WARN:          {3}" + Environment.NewLine +
                        "ERROR:         {4}" + Environment.NewLine +
                        "FATAL:         {5}" + Environment.NewLine +
                        "Total Entries: {6}" + Environment.NewLine +
                        "First Entry:   {7}" + Environment.NewLine +
                        "Last Entry:    {8}",
                        _logFiles.Sum(l => l.EntryStats.Trace),
                        _logFiles.Sum(l => l.EntryStats.Debug),
                        _logFiles.Sum(l => l.EntryStats.Info),
                        _logFiles.Sum(l => l.EntryStats.Warn),
                        _logFiles.Sum(l => l.EntryStats.Error),
                        _logFiles.Sum(l => l.EntryStats.Fatal),
                        _logFiles.Sum(l => l.EntryStats.TotalEntries),
                        _logFiles.Min(l => l.EntryStats.FirstEntry),
                        _logFiles.Max(l => l.EntryStats.LastEntry));
                    
                    break;
            }

            logValueTextBox.Text = text;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contexts"></param>
        /// <param name="formatContextEnum"></param>
        /// <param name="indentation"></param>
        /// <returns></returns>
        private static string GetContextText(ContextCollection contexts, FormatContextEnum formatContextEnum, int indentation) {
            var context = contexts[formatContextEnum];
            if (context == null)
                return null;

            var padding = GetPadding(indentation*3);
            return String.Format(
                "{0}{3}" + Environment.NewLine +
                "{1}Regex:" + Environment.NewLine +
                "{2}{4}" + Environment.NewLine +
                "{1}Fields:" + Environment.NewLine +
                "{5}" + Environment.NewLine,
                GetPadding(indentation),
                GetPadding(indentation * 2),
                padding,
                formatContextEnum,
                context.Regex,
                GetFormatFieldText(context.Fields, padding));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatFields"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        private static string GetFormatFieldText(FormatFieldCollection formatFields, string padding) {
            var len = formatFields.Count;
            var sb = new StringBuilder();

            for (var i = 0; i < len; i++) {
                var formatField = formatFields[i];

                sb.AppendFormat(
                    "{0}[{1}]  Id:              {2}" + Environment.NewLine +
                    "{0}     Name:            {3}" + Environment.NewLine +
                    "{0}     Display:         {4}" + Environment.NewLine +
                    "{0}     Data Type:       {5}" + Environment.NewLine +
                    "{0}     Required:        {6}" + Environment.NewLine +
                    "{0}     Default:         {7}" + Environment.NewLine +
                    "{0}     Filterable:      {8}" + Environment.NewLine +
                    "{0}     Format Pattern:  {9}" + Environment.NewLine +
                    "{0}     Groups:          {10}" + Environment.NewLine,
                    padding,
                    i,
                    formatField.Id,
                    formatField.Name,
                    formatField.Display,
                    formatField.DataType,
                    formatField.Required,
                    formatField.Default,
                    formatField.Filterable,
                    formatField.FormatPattern,
                    String.Join(", ", formatField.Groups));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string GetPadding(int len) {
            var chars = new string[len];
            for (var i = 0; i < len; i++)
                chars[i] = " ";

            return String.Join("", chars);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueFunc"></param>
        /// <param name="nameFunc"></param>
        /// <returns></returns>
        private string GetAggregateProperty<T>(Func<LogFile, T> valueFunc, Func<T, string> nameFunc) {
            if (_logFiles.Length == 1)
                return nameFunc(valueFunc(_logFiles[0]));

            var counts = new Dictionary<T, AggregateProperty>();
            var width = 10;

            for (var i = 0; i < _logFiles.Length; i++) {
                var item = valueFunc(_logFiles[i]);
                var name = nameFunc(item);
                AggregateProperty agg;

                if (name.Length > width)
                    width = name.Length;

                if (counts.TryGetValue(item, out agg))
                    counts[item] = new AggregateProperty(name, agg.Count + 1);
                else
                    counts.Add(item, new AggregateProperty(name, 1));
            }

            var sb = new StringBuilder();
            var format = "{0,-" + width + "}: {1}{2}" + Environment.NewLine;

            foreach (var value in counts.Values)
                sb.AppendFormat(format, value.Name, value.Count, value.Count == 1 ? " log" : " logs");

            return sb.ToString();
        }
        
        #endregion
    }
}