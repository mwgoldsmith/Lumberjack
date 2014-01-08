using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medidata.Lumberjack.Core;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Data.Collections;
using Medidata.Lumberjack.Core.Processing;

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

        protected struct PropertyItem
        {
            public string DisplayText { get; set; }    
        }

        #endregion

        #region Private fields

        private readonly object[] _standardProperties = new object[]
            {
                "Full Filename",
                "Filesize",
                "Session Format",
                "Hashing Status",
                "Filename Parsing Status",
                "Entry Parsing Status",
                "Entry Statistics"
            };

        private readonly object[] _singleProperties = new object[]
            {
                "ID",
                "MD5 Hash"
            };

        private readonly LogFile[] _logFiles;
        private readonly UserSession _session;

        private readonly Dictionary<SessionFormat, List<FormatField>> _sessionFormats = new Dictionary<SessionFormat, List<FormatField>>();
        private readonly List<SessionField> _sessionFields = new List<SessionField>();

        #endregion

        #region Initializers

        /// <summary>
        /// Creates a new LogPropertiesForm instance
        /// </summary>
        /// <param name="session"></param>
        /// <param name="logFiles"></param>
        public LogPropertiesForm(UserSession session, LogFile[] logFiles) {
            InitializeComponent();

            _logFiles = logFiles;
            _session = session;

            // Determine all Session and Format Fields within the LogFiles
            foreach (var l in _logFiles) {
                var sessionFormat = l.SessionFormat;

                // If SessionFormat is not known, the fields are not known either 
                if (sessionFormat == null)
                    continue;

                // Only FormatFields within the Filename context are relevant
                var items = sessionFormat.Contexts[FormatContextEnum.Filename].Fields.ToList();
                List<FormatField> formatFields;

                if (!_sessionFormats.TryGetValue(sessionFormat, out formatFields)) {
                    formatFields = new List<FormatField>();
                    _sessionFormats.Add(sessionFormat, formatFields);
                }

                foreach (var f in items) {
                    if (!_sessionFields.Contains(f.SessionField))
                        _sessionFields.Add(f.SessionField);

                    if (!formatFields.Contains(f))
                        formatFields.Add(f);
                }
            }
        }

        #endregion

        #region Form event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogPropertiesForm_Load(object sender, EventArgs e) {
            // Add properties to list box
            if (_logFiles.Length == 1) {
                logFileTextBox.Text = _logFiles[0].Filename;
                logPropertyListBox.Items.AddRange(_singleProperties);
            } else {
                logFileTextBox.Text = _logFiles.Length + " file(s)";
            }

            logPropertyListBox.Items.AddRange(_standardProperties);
            
            // Add all SessionFields within the LogFiles
            sessionFieldListBox.DataSource = _sessionFields;
            sessionFieldListBox.DisplayMember = "Display";
            
            // Add all FormatFields
            var treeNodes = new List<TreeNode>();
            foreach (var s in _sessionFormats) {
                var childNodes = s.Value.Select(formatField => new TreeNode(formatField.Name)).ToArray();
                treeNodes.Add(new TreeNode(s.Key.Name, childNodes));
            }

            formatFieldTreeView.Nodes.AddRange(treeNodes.ToArray());
            formatFieldTreeView.ExpandAll();

            // Select first property and default field level
            logPropertyListBox.SelectedIndices.Add(0);
            fieldLevelComboBox.SelectedIndex = 0;
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
        private void editButton_Click(object sender, EventArgs e) {
            editPanel.Height = 65;
            logValueTextBox.Top = editPanel.Bottom + editPanel.Margin.Top + editPanel.Margin.Bottom;
            logValueTextBox.Height -= 28;

            logPropertyListBox.Enabled = false;
            fieldLevelComboBox.Enabled = false;
            sessionFieldListBox.Enabled = false;
            formatFieldTreeView.Enabled = false;
            editButton.Enabled = false;
            valueTextBox.ReadOnly = false;
            valueComboBox.Enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, EventArgs e) {
            // TODO: Save
            cancelButton.PerformClick();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e) {
            editPanel.Height = 37;
            logValueTextBox.Top = editPanel.Bottom + editPanel.Margin.Top + editPanel.Margin.Bottom;
            logValueTextBox.Height += 28;

            logPropertyListBox.Enabled = true;
            fieldLevelComboBox.Enabled = true;
            sessionFieldListBox.Enabled = true;
            formatFieldTreeView.Enabled = true;
            saveButton.Enabled = false;
            editButton.Enabled = true;
            valueTextBox.ReadOnly = true;
            valueComboBox.Enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logPropertyListBox_SelectedIndexChanged(object sender, EventArgs e) {
            var selected = logPropertyListBox.SelectedItem.ToString();
            var detailText = "";
            string textValue = null;
            var editable = false;

            switch (selected) {
                case "ID":
                    if (_logFiles.Length == 1)
                        textValue = _logFiles[0].Id.ToString();

                    break;
                case "MD5 Hash":
                    if (_logFiles.Length == 1)
                        textValue = _logFiles[0].Md5Hash;

                    break;

                case "Filesize":
                    textValue = _logFiles.Sum(l => l.Filesize) + " byte(s)";
                    break;

                case "Full Filename":
                    if (_logFiles.Length == 1) {
                        editable = true;
                        textValue = _logFiles[0].FullFilename;
                    } else {
                        var sb = new StringBuilder();
                        foreach (var l in _logFiles)
                            sb.Append(l.FullFilename + Environment.NewLine);

                        detailText = sb.ToString();
                    }

                    break;
                case "Session Format":
                    valueComboBox.SuspendLayout();
                    foreach (var s in _session.Formats)
                        valueComboBox.Items.Add(s.Name);

                    valueComboBox.ResumeLayout(true);

                    if (_logFiles.Length == 1) {
                        var sessionFormat = _logFiles[0].SessionFormat;

                        // For now, the session format is only editable if it is underdermined
                        editable = sessionFormat == null;

                        if (sessionFormat == null) {
                            valueComboBox.Text = "(unknown)";
                        } else {
                            detailText = String.Format(
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
                                detailText += contextText;
                            contextText = GetContextText(sessionFormat.Contexts, FormatContextEnum.Entry, 2);
                            if (contextText != null)
                                detailText += contextText;
                            contextText = GetContextText(sessionFormat.Contexts, FormatContextEnum.Content, 2);
                            if (contextText != null)
                                detailText += contextText;

                            valueComboBox.SelectedItem = sessionFormat.Name;
                        }
                    } else {
                        detailText = GetAggregateProperty(v => v.SessionFormat, n => n.Name);
                        valueComboBox.Text = "(multiple)";
                    }


                    break;
                case "Hashing Status":
                    PopulateEngineStatusCombo(valueComboBox);
                    if (_logFiles.Length == 1) {
                        valueComboBox.SelectedItem = _logFiles[0].HashStatus.ToString();
                    } else {
                        detailText = GetAggregateProperty(v => v.HashStatus, n => n.ToString());
                    }

                    editable = true;

                    break;
                case "Filename Parsing Status":
                    PopulateEngineStatusCombo(valueComboBox);
                    if (_logFiles.Length == 1) {
                        valueComboBox.SelectedItem = _logFiles[0].FilenameParseStatus.ToString();
                    } else {
                        detailText = GetAggregateProperty(v => v.FilenameParseStatus, n => n.ToString());
                    }

                    editable = true;

                    break;
                case "Entry Parsing Status":
                    PopulateEngineStatusCombo(valueComboBox);
                    if (_logFiles.Length == 1) {
                        valueComboBox.SelectedItem = _logFiles[0].EntryParseStatus.ToString();
                    } else {
                        detailText = GetAggregateProperty(v => v.EntryParseStatus, n => n.ToString());
                    }

                    editable = true;
                    break;
                case "Entry Statistics":
                    detailText = String.Format(
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

            editButton.Enabled = editable;

            valueTextBox.Visible = (textValue != null);
            valueComboBox.Visible = (textValue == null);

            logValueTextBox.Text = detailText;
            if (textValue != null)
                valueTextBox.Text = textValue;
            else {
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fieldLevelComboBox_SelectedValueChanged(object sender, EventArgs e) {
            var control = (ComboBox)sender;
            var sessionLevel = control.SelectedItem.ToString().Equals("Session Level");

            sessionFieldListBox.Visible = sessionLevel;
            formatFieldTreeView.Visible = !sessionLevel;

            // if (sessionLevel)
            //     sessionFieldListBox.SelectedIndex = 0;
            // else 
            //     formatFieldTreeView.se?
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sessionFieldListBox_SelectedIndexChanged(object sender, EventArgs e) {
            var control = (ListBox) sender;
            var sessionField = control.SelectedItem as SessionField;
            var detailText = GetSessionFieldText(sessionField, GetPadding(2));


            if (_logFiles.Length == 1) {
                valueTextBox.Text  = _session.FieldValues.Find(_logFiles[0], _logFiles[0].SessionFormat.Contexts[FormatContextEnum.Filename].Fields.Find(sessionField).ToArray()[0]).ToString();
            } else {
                
            }

            valueTextBox.Visible = true;
            valueComboBox.Visible = false;

            logValueTextBox.Text = detailText;
        }

        private void formatFieldTreeView_LocationChanged(object sender, EventArgs e) {

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
        /// <param name="sessionField"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        private static string GetSessionFieldText(SessionField sessionField, string padding) {
            var contexts = "";

            if (sessionField.ContextFlags == FieldContextFlags.None) {
                contexts = contexts + padding + "(none)";
            } else {
                var values = Enum.GetValues(typeof (FieldContextFlags));

                foreach (int e in values) {
                    if (e != 0 && ((int) sessionField.ContextFlags & e) == e) 
                        contexts = contexts + padding + Enum.ToObject(typeof (FieldContextFlags), e) + Environment.NewLine;
                }
            }

            return String.Format(
                    "Id:              {0}" + Environment.NewLine +
                    "Name:            {1}" + Environment.NewLine +
                    "Display:         {2}" + Environment.NewLine +
                    "Data Type:       {3}" + Environment.NewLine +
                    "Required:        {4}" + Environment.NewLine +
                    "Default:         {5}" + Environment.NewLine +
                    "Filterable:      {6}" + Environment.NewLine +
                    "Format Pattern:  {7}" + Environment.NewLine +
                    "Contexts:        " + Environment.NewLine + "{8}",
                    sessionField.Id,
                    sessionField.Name,
                    sessionField.Display,
                    sessionField.DataType,
                    sessionField.Required,
                    sessionField.Default,
                    sessionField.Filterable,
                    sessionField.FormatPattern,
                    contexts);
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
                if (formatField == null) {
                    var x = formatField;
                }
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
                    formatField.Groups == null ? "" : String.Join(", ", formatField.Groups));
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        private static void PopulateEngineStatusCombo(ComboBox control) {
            control.SuspendLayout();

            control.Items.Clear();
            foreach (var s in Enum.GetNames(typeof (EngineStatusEnum)))
                control.Items.Add(s);

            control.ResumeLayout(true);
        }

        #endregion
    }
}