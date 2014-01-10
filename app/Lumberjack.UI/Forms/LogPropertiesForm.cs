using System;
using System.Collections.Generic;
using System.Drawing;
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
        #region Nested structs and classes

        /// <summary>
        /// 
        /// </summary>
        protected struct AggregateProperty
        {
            #region Initializers

            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="count"></param>
            public AggregateProperty(string name, int count) : this() {
                Name = name;
                Count = count;
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
            public int Count { get; set; }

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        protected class EngineStatusProperty
        {
            #region Initializers

            /// <summary>
            /// 
            /// </summary>
            /// <param name="status"></param>
            public EngineStatusProperty(EngineStatusEnum status) {
                Status = status;
                Name = status.ToString();
                Value = (int) status;
            }

            #endregion

            #region Properties

            /// <summary>
            /// 
            /// </summary>
            public EngineStatusEnum Status { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            public int Value { get; private set; }

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        protected class ListItem
        {
            #region Initializers

            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="value"></param>
            public ListItem(string name, object value) {
                Name = name;
                Value = value;
                Readonly = true;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            public ListItem(string name)
                : this(name, null) {
            }

            /// <summary>
            /// 
            /// </summary>
            public ListItem()
                : this(null) {
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
            public string Details { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public object Value { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public object Data { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public object ComboDataSource { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string ComboDisplayMember { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public bool Readonly { get; set; }

            #endregion
        }

        #endregion

        #region Private fields

        private static readonly List<EngineStatusProperty> _engineStatusProperties;

        private readonly LogFile[] _logFiles;
        private readonly UserSession _session;

        private readonly List<ListItem> _propertyItems;
        private readonly List<ListItem> _sessionFieldItems;
        private readonly Dictionary<SessionFormat, List<ListItem>> _formatFieldItems;

        private bool _isFieldSelected;
        private bool _isFormatLevelFields;
        private ListItem _curListItem;
        private bool _editMode;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        static LogPropertiesForm() {
            _engineStatusProperties = new List<EngineStatusProperty>
                {
                    new EngineStatusProperty(EngineStatusEnum.None),
                    new EngineStatusProperty(EngineStatusEnum.Processing),
                    new EngineStatusProperty(EngineStatusEnum.Completed),
                    new EngineStatusProperty(EngineStatusEnum.Failed)
                };
        }
    
        /// <summary>
        /// Creates a new LogPropertiesForm instance
        /// </summary>
        /// <param name="session"></param>
        /// <param name="logFiles"></param>
        public LogPropertiesForm(UserSession session, LogFile[] logFiles) {
            InitializeComponent();

            _logFiles = logFiles;
            _session = session;

            _propertyItems = CreatePropertyItems(_logFiles);
            _sessionFieldItems = CreateSessionFieldItems(_logFiles);
            _formatFieldItems = CreateFormatFieldItems(_logFiles);
        }

        #endregion

        #region Form event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogPropertiesForm_Load(object sender, EventArgs e) {
            var singleView = _logFiles.Length == 1;

            logFileTextBox.Text = singleView ? _logFiles[0].Filename : (_logFiles.Length + " file(s)");

            // Add properties to list box
            logPropertyListBox.DataSource = _propertyItems;
            logPropertyListBox.DisplayMember = "Name";
            
            // Add all SessionFields within the LogFiles
            sessionFieldListBox.DataSource = _sessionFieldItems;
            sessionFieldListBox.DisplayMember = "Name";
            
            // Add all FormatFields
            var treeNodes = new List<TreeNode>();
            foreach (var s in _formatFieldItems) {
                var listItems = s.Value;

                var childNodes = new TreeNode[listItems.Count];
                for (var i = 0; i < listItems.Count; i++)
                    childNodes[i] = new TreeNode(listItems[i].Name) { Tag = listItems[i] };

                treeNodes.Add(new TreeNode(s.Key.Name, childNodes));
            }

            formatFieldTreeView.Nodes.AddRange(treeNodes.ToArray());
            formatFieldTreeView.ExpandAll();

            // Select first property and default field level
            fieldLevelComboBox.SelectedIndex = 0;
            logPropertyListBox.SelectedIndices.Add(0);

            SelectDataItem(logPropertyListBox.SelectedItem as ListItem, false);
        }

        #endregion

        #region Button event handlers

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
            valueComboBox.DropDownStyle =_isFieldSelected ? ComboBoxStyle.DropDown : ComboBoxStyle.DropDownList;

            _editMode = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, EventArgs e) {
            var value = _curListItem.ComboDataSource == null ? valueTextBox.Text : valueComboBox.Text;
            var result = MessageBox.Show("Are you sure you want to set the value of " + _curListItem.Name + " to \"" + value + "\" for the selected log file(s)?", 
                "Confirm change",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.No) 
                return;

            if (_isFieldSelected) {
                // Update the modified field value for the referenced LogFile objects

                FormatField formatField = null;
                SessionField sessionField = null;

                if (_isFormatLevelFields) {
                    formatField = _curListItem.Data as FormatField;
                } else {
                    sessionField = _curListItem.Data as SessionField;
                }

                foreach (var logFile in _logFiles) {
                    // When the change is being applied at a session field level, we would need to find the
                    // linked format field for each logfile.
                    if (sessionField != null)
                        formatField = logFile.SessionFormat.Contexts[FormatContextEnum.Filename].Fields.FindFirst(sessionField);

                    if (formatField == null) {
                        // TODO: this log does not contains a format field linked to the selected session field. Notify user
                    } else {
                        // Add value if non-existant; else, update
                        _session.FieldValues.Update(logFile, formatField, value);
                    }
                }
            } else {
                // Update the log file property for the referenced LogFile objects
                var logFiles = new List<LogFile>(_logFiles);

                switch (_curListItem.Name) {
                    case "Full Filename":
                        break;
                    case "Session Format":
                        var sessionFormat = valueComboBox.SelectedItem as SessionFormat;
                        logFiles.ForEach(x => x.SessionFormat = sessionFormat);
                        break;
                    case "Hashing Status":
                        break;
                    case "Filename Parsing Status":
                        break;
                    case "Entry Parsing Status":
                        break;
                }

                _session.LogFiles.Update(_logFiles);
            }

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
            valueComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            valueComboBox.SelectedItem = _curListItem.Value;

            _editMode = false;
        }

        #endregion

        #region Other control event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logPropertyListBox_SelectedIndexChanged(object sender, EventArgs e) {
            SelectDataItem(((ListBox)sender).SelectedItem as ListItem, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logPropertyListBox_DrawItem(object sender, DrawItemEventArgs e) {
            DrawListBoxItem((ListBox)sender, e, !_isFieldSelected);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fieldLevelComboBox_SelectedValueChanged(object sender, EventArgs e) {
            var control = (ComboBox)sender;
            _isFormatLevelFields = control.SelectedItem.ToString().Equals("Format Level");

            sessionFieldListBox.Visible = !_isFormatLevelFields;
            formatFieldTreeView.Visible = _isFormatLevelFields;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sessionFieldListBox_SelectedIndexChanged(object sender, EventArgs e) {
            SelectDataItem(((ListBox) sender).SelectedItem as ListItem, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sessionFieldListBox_DrawItem(object sender, DrawItemEventArgs e) {
            DrawListBoxItem((ListBox)sender, e, _isFieldSelected);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formatFieldTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
            SelectDataItem(e.Node.Tag as ListItem, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void valueTextBox_TextChanged(object sender, EventArgs e) {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void valueComboBox_SelectedValueChanged(object sender, EventArgs e) {
            var selected = valueComboBox.SelectedItem;

            if (!_isFieldSelected && valueComboBox.DataSource != null && String.IsNullOrWhiteSpace(valueComboBox.DisplayMember))
                return;

            var modified = _curListItem == null
                            || (_curListItem.Value == null && selected != null)
                            || (_isFieldSelected
                                    ? !Equals(_curListItem.Value, selected)
                                    : !ReferenceEquals(_curListItem.Value, selected));

            saveButton.Enabled = modified;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void valueComboBox_TextChanged(object sender, EventArgs e) {
            var text = valueComboBox.Text;
            var modified = false;

            if (_curListItem != null && _curListItem.Value == null) {
                modified = !string.IsNullOrEmpty(text);
            } else if (_curListItem != null) {
                if (_isFieldSelected)
                    modified = !Equals(_curListItem.Value, text);
                else
                    modified = !ReferenceEquals(_curListItem.Value, text);
            }

            saveButton.Enabled = modified;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="e"></param>
        /// <param name="focused"></param>
        private static void DrawListBoxItem(ListBox control, DrawItemEventArgs e, bool focused) {
            e.DrawBackground();

            var isItemSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
            var itemIndex = e.Index;

            if (itemIndex >= 0 && itemIndex < control.Items.Count) {
                var g = e.Graphics;

                // Background Color
                var color = isItemSelected ? (focused ? SystemColors.Highlight : SystemColors.InactiveCaption) : Color.White;
                using (var backgroundColorBrush = new SolidBrush(color)) {
                    g.FillRectangle(backgroundColorBrush, e.Bounds);

                    // Set text color
                    var item = control.Items[itemIndex] as ListItem;
                    var itemText = item != null ? item.Name : "";

                    color = isItemSelected ? (focused ? SystemColors.HighlightText : SystemColors.InactiveCaptionText) : SystemColors.WindowText;
                    using (var itemTextColorBrush = new SolidBrush(color))
                        g.DrawString(itemText, e.Font, itemTextColorBrush, control.GetItemRectangle(itemIndex).Location);
                }
            }

            e.DrawFocusRectangle();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selected"></param>
        /// <param name="fieldSelected"></param>
        private void SelectDataItem(ListItem selected, bool fieldSelected) {
            var textOnly = selected == null || selected.ComboDataSource == null;
            var logValueText = selected == null ? "" : selected.Details;
            var valueText = selected == null || (!textOnly || selected.Value == null) ? "" : selected.Value.ToString();

            _isFieldSelected = fieldSelected;
            _curListItem = selected;

            sessionFieldListBox.Refresh();
            logPropertyListBox.Refresh();

            editButton.Enabled = selected != null && !selected.Readonly;

            logValueTextBox.Text = logValueText;

            valueTextBox.Text = valueText;
            valueTextBox.Visible = textOnly;

            if (textOnly) {
                valueComboBox.DataSource = null;
                valueComboBox.DisplayMember = null;
            } else {
                valueComboBox.SelectedItem = null;
                valueComboBox.DataSource = selected.ComboDataSource;
                valueComboBox.DisplayMember = selected.ComboDisplayMember;
                valueComboBox.SelectedItem = selected.Value;
            }

            valueComboBox.Visible = !textOnly;
            valueComboBox.Enabled = _editMode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFiles"></param>
        /// <returns></returns>
        private List<ListItem> CreatePropertyItems(LogFile[] logFiles) {
            var result = new List<ListItem>();
            var singleView = logFiles.Length == 1;
            var logFile = logFiles[0];
            Func<EngineStatusEnum, object> getEngineStatusDropdown = x =>
                                                                     (object) _engineStatusProperties.Find(y => (EngineStatusEnum) y.Value == x);

            if (singleView) {
                result.Add(new ListItem("ID", logFile.Id.ToString()));
                result.Add(new ListItem("MD5 Hash", logFile.Md5Hash));
            }

            result.AddRange(new[]
                {
                    new ListItem("Filesize", logFiles.Sum(l => l.Filesize) + " byte(s)"),
                    new ListItem("Full Filename", singleView ? logFile.FullFilename : null)
                        {
                            Details = GetFullFilenamePropertyDetail(logFiles),
                            Readonly = !singleView
                        },
                    new ListItem("Session Format")
                        {
                            Value = TestAllValuesSame(logFiles, v => v.SessionFormat == null ? -1 : v.SessionFormat.Id) ? logFile.SessionFormat : null,
                            Details = GetSessionFormatPropertyDetail(logFiles),
                            Readonly = !singleView || logFile.SessionFormat != null,
                            ComboDataSource = _session.Formats.ToList(),
                            ComboDisplayMember = "Name"
                        },
                    new ListItem("Hashing Status")
                        {
                            Value = TestAllValuesSame(logFiles, v => (int) v.HashStatus) ? getEngineStatusDropdown(logFile.HashStatus) : null,
                            Details = singleView ? null : GetAggregateProperty(v => v.HashStatus, n => n.ToString()),
                            ComboDataSource = _engineStatusProperties,
                            ComboDisplayMember = "Name",
                            Readonly = false
                        },
                    new ListItem("Filename Parsing Status")
                        {
                            Value = TestAllValuesSame(logFiles, v => (int) v.FilenameParseStatus) ? getEngineStatusDropdown(logFile.FilenameParseStatus) : null,
                            Details = singleView ? null : GetAggregateProperty(v => v.FilenameParseStatus, n => n.ToString()),
                            ComboDataSource = _engineStatusProperties,
                            ComboDisplayMember = "Name",
                            Readonly = false
                        },
                    new ListItem("Entry Parsing Status")
                        {
                            Value = TestAllValuesSame(logFiles, v => (int) v.EntryParseStatus) ? getEngineStatusDropdown(logFile.EntryParseStatus) : null,
                            Details = singleView ? null : GetAggregateProperty(v => v.EntryParseStatus, n => n.ToString()),
                            ComboDataSource = _engineStatusProperties,
                            ComboDisplayMember = "Name",
                            Readonly = false
                        },
                    new ListItem("Entry Statistics")
                        {
                            Details = GetEntryStatsDetail(_logFiles)
                        }
                });

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFiles"></param>
        /// <returns></returns>
        private List<ListItem> CreateSessionFieldItems(LogFile[] logFiles) {
            var result = new List<ListItem>();

            // Determine all Session and Format Fields within the LogFiles
            foreach (var logFile in logFiles) {
                var sessionFormat = logFile.SessionFormat;

                // If SessionFormat is not known, the fields are not known either 
                if (sessionFormat == null)
                    continue;

                // Only FormatFields within the Filename context are relevant
                var formatFields = sessionFormat.Contexts[FormatContextEnum.Filename].Fields;

                foreach (var formatField in formatFields) {
                    var sessionField = formatField.SessionField;
                    if (result.Exists(x => ReferenceEquals(x.Data as SessionField, sessionField)))
                        continue;

                    // Get the value of the field to be used for the ListItem
                    // Only use the field value if every logfile which has a format field
                    // linked to the same session field also has the same field value.
                    var value = _session.FieldValues.Find(logFile, formatField);
                    if (value == null || !TestAllFieldValues(logFiles, sessionField, value.ToString()))
                        value = null;

                    result.Add(new ListItem(sessionField.Display)
                        {
                            Data = sessionField,
                            Value = value,
                            Details = GetSessionFieldText(sessionField, new String(' ', 2)),
                            ComboDataSource = _session.FieldValues.FindAll(sessionField).Distinct().ToList(),
                            Readonly = false
                        });
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFiles"></param>
        /// <returns></returns>
        private Dictionary<SessionFormat, List<ListItem>> CreateFormatFieldItems(LogFile[] logFiles) {
            var result = new Dictionary<SessionFormat, List<ListItem>>();

            // Determine all Session and Format Fields within the LogFiles
            foreach (var logFile in logFiles) {
                var sessionFormat = logFile.SessionFormat;

                // If SessionFormat is not known, the fields are not known either 
                if (sessionFormat == null)
                    continue;

                // Only FormatFields within the Filename context are relevant
                var items = sessionFormat.Contexts[FormatContextEnum.Filename].Fields;
                List<ListItem> formatFields;

                if (!result.TryGetValue(sessionFormat, out formatFields)) {
                    formatFields = new List<ListItem>();
                    result.Add(sessionFormat, formatFields);
                }

                foreach (var f in items) {
                    if (formatFields.Count > 0 && formatFields.Exists(x => ReferenceEquals(x.Data as FormatField, f)))
                        continue;

                    // Get the value of the field to be used for the ListItem
                    // Only use the field value if every logfile which has a format field
                    // linked to the same session field also has the same field value.
                    var value = _session.FieldValues.Find(logFile, f);
                    if (value == null || !TestAllFieldValues(logFiles, f, value.ToString()))
                        value = null;

                    formatFields.Add(new ListItem(f.Display)
                    {
                        Data = f,
                        Value = value,
                        Details = GetFormatFieldText(f),
                        ComboDataSource = _session.FieldValues.FindAll(f).Distinct().ToList(),
                        Readonly = false
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFiles"></param>
        /// <param name="sessionField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool TestAllFieldValues(LogFile[] logFiles, SessionField sessionField, string value) {
            for (var i = logFiles.Length - 1; i >= 0; i--) {
                var logFile = logFiles[i];
                var sessionFormat = logFile.SessionFormat;

                if (sessionFormat == null)
                    continue;

                var f = sessionFormat.Contexts[FormatContextEnum.Filename].Fields;
                var formatField = f.FindFirst(sessionField);
                if (formatField == null)
                    continue;

                var fieldValue = _session.FieldValues.Find(logFile, formatField);
                if (fieldValue == null || !value.Equals(fieldValue.ToString()))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFiles"></param>
        /// <param name="formatField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool TestAllFieldValues(LogFile[] logFiles, FormatField formatField, string value) {
            for (var i = logFiles.Length - 1; i >= 0; i--) {
                var logFile = logFiles[i];
                var sessionFormat = logFile.SessionFormat;

                if (sessionFormat == null)
                    continue;

                if (!sessionFormat.Contexts[FormatContextEnum.Filename].Fields.Contains(formatField))
                    continue;

                var fieldValue = _session.FieldValues.Find(logFile, formatField);
                if (fieldValue == null || !value.Equals(fieldValue.ToString()))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if a value is the same for all of the given LogFiles by calling the specified
        /// predicate.
        /// </summary>
        /// <param name="logFiles">Array of LogFile objects to pass to the predicate.</param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private static bool TestAllValuesSame(LogFile[] logFiles, Func<LogFile, int> predicate) {
            if (logFiles.Length == 1)
                return true;

            var value = predicate(logFiles[0]);

            return logFiles.All(t => predicate(t) == value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueFunc"></param>
        /// <param name="nameFunc"></param>
        /// <returns></returns>
        private string GetAggregateProperty<T>(Func<LogFile, T> valueFunc, Func<T, string> nameFunc) {
            var len = _logFiles.Length;
            if (len == 1)
                return nameFunc(valueFunc(_logFiles[0]));

            var counts = new Dictionary<T, AggregateProperty>();
            var width = 10;

            // Count the number of occurances of a property by value through all
            // referenced LogFile objects
            for (var i = 0; i < len; i++) {
                var item = valueFunc(_logFiles[i]);
                if (Equals(item, default(T)))
                    continue;

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

        #region Private methods for generating formatted detail text

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFiles"></param>
        /// <returns></returns>
        private static string GetFullFilenamePropertyDetail(LogFile[] logFiles) {
            // Property 'Full Filename' should only have detail text when in MULTI view
            if (logFiles.Length == 1)
                return null;

            var sb = new StringBuilder();
            foreach (var l in logFiles)
                sb.Append(l.FullFilename + Environment.NewLine);

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFiles"></param>
        /// <returns></returns>
        private static string GetEntryStatsDetail(LogFile[] logFiles) {
            return String.Format(
                "TRACE:         {0}" + Environment.NewLine +
                "DEBUG:         {1}" + Environment.NewLine +
                "INFO:          {2}" + Environment.NewLine +
                "WARN:          {3}" + Environment.NewLine +
                "ERROR:         {4}" + Environment.NewLine +
                "FATAL:         {5}" + Environment.NewLine +
                "Total Entries: {6}" + Environment.NewLine +
                "First Entry:   {7}" + Environment.NewLine +
                "Last Entry:    {8}",
                logFiles.Sum(l => l.EntryStats.Trace),
                logFiles.Sum(l => l.EntryStats.Debug),
                logFiles.Sum(l => l.EntryStats.Info),
                logFiles.Sum(l => l.EntryStats.Warn),
                logFiles.Sum(l => l.EntryStats.Error),
                logFiles.Sum(l => l.EntryStats.Fatal),
                logFiles.Sum(l => l.EntryStats.TotalEntries),
                logFiles.Min(l => l.EntryStats.FirstEntry),
                logFiles.Max(l => l.EntryStats.LastEntry));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFiles"></param>
        /// <returns></returns>
        private string GetSessionFormatPropertyDetail(LogFile[] logFiles) {
            if (logFiles.Length != 1) 
                return GetAggregateProperty(v => v.SessionFormat, n => n != null ? n.Name  : "");
            
            var sessionFormat = logFiles[0].SessionFormat;

            // For now, the session format is only editable if it is underdermined
            //editable = sessionFormat == null;

            if (sessionFormat != null) {
               // return null; //valueComboBox.Text = "(unknown)";
          
                var sb = new StringBuilder();
                sb.AppendFormat(
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
                    sb.Append(contextText);
                contextText = GetContextText(sessionFormat.Contexts, FormatContextEnum.Entry, 2);
                if (contextText != null)
                    sb.Append(contextText);
                contextText = GetContextText(sessionFormat.Contexts, FormatContextEnum.Content, 2);
                if (contextText != null)
                    sb.Append(contextText);

                //valueComboBox.SelectedItem = sessionFormat.Name;

                return sb.ToString();
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contexts"></param>
        /// <param name="formatContextEnum"></param>
        /// <param name="indentation"></param>
        /// <returns></returns>
        private string GetContextText(ContextCollection contexts, FormatContextEnum formatContextEnum, int indentation) {
            var context = contexts[formatContextEnum];
            if (context == null)
                return null;

            var padding = new String (' ', indentation*3);
            return String.Format(
                "{0}{3}" + Environment.NewLine +
                "{1}Regex:" + Environment.NewLine +
                "{2}{4}" + Environment.NewLine +
                "{1}Fields:" + Environment.NewLine +
                "{5}" + Environment.NewLine,
                new String (' ', indentation),
                new String (' ', indentation * 2),
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
        /// <param name="formatField"></param>
        /// <returns></returns>
        private static string GetFormatFieldText(FormatField formatField) {
            return String.Format(
                    "Id:              {0}" + Environment.NewLine +
                    "Name:            {1}" + Environment.NewLine +
                    "Display:         {2}" + Environment.NewLine +
                    "Data Type:       {3}" + Environment.NewLine +
                    "Required:        {4}" + Environment.NewLine +
                    "Default:         {5}" + Environment.NewLine +
                    "Filterable:      {6}" + Environment.NewLine +
                    "Format Pattern:  {7}" + Environment.NewLine +
                    "Context:         {8}" + Environment.NewLine +
                    "Session Format:  {9}" + Environment.NewLine,
                    formatField.Id,
                    formatField.Name,
                    formatField.Display,
                    formatField.DataType,
                    formatField.Required,
                    formatField.Default,
                    formatField.Filterable,
                    formatField.FormatPattern,
                    formatField.Type,
                    formatField.SessionFormat.Name);
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
                    formatField.Groups == null ? "" : String.Join(", ", formatField.Groups));
            }

            return sb.ToString();
        }

        #endregion

    }
}