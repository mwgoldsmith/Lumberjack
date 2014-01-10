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
        protected struct EngineStatusProperty
        {
            #region Initializers

            /// <summary>
            /// 
            /// </summary>
            /// <param name="status"></param>
            public EngineStatusProperty(EngineStatusEnum status)
                : this() {
                Status = status;
                Name = status.ToString();
                Value = (int)status;
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
            logPropertyListBox.SelectedIndices.Add(0);
            fieldLevelComboBox.SelectedIndex = 0;

            // Set focus to the log property listbox control, and blue the field controls
            _isFieldSelected = false;
            sessionFieldListBox.Refresh();
            logPropertyListBox.Refresh();
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
            valueComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        #endregion

        #region Other control event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logPropertyListBox_SelectedIndexChanged(object sender, EventArgs e) {
            var control = (ListBox)sender;

            SelectDataItem(control.SelectedItem as ListItem, false);
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

            SelectDataItem(control.SelectedItem as ListItem, true);
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

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void valueComboBox_TextChanged(object sender, EventArgs e) {

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
            var valueText = selected == null || !textOnly ? "" : selected.Value.ToString();

            _isFieldSelected = fieldSelected;
            sessionFieldListBox.Refresh();
            logPropertyListBox.Refresh();

            editButton.Enabled = selected != null && !selected.Readonly;

            logValueTextBox.Text = logValueText;

            valueTextBox.Text = valueText;
            valueTextBox.Visible = textOnly;

            valueComboBox.Visible = !textOnly;

            if (textOnly) {
                valueComboBox.DataSource = null;
                valueComboBox.DisplayMember = null;
            } else {
                valueComboBox.DataSource = selected.ComboDataSource;
                valueComboBox.DisplayMember = selected.ComboDisplayMember;
                valueComboBox.SelectedItem = selected.Value;
            }
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
                (object) _engineStatusProperties.Find(y => (EngineStatusEnum) y.Value == logFile.EntryParseStatus);

            if (singleView) {
                result.Add(new ListItem("ID", logFile.Id.ToString()));
                result.Add(new ListItem("MD5 Hash", logFile.Md5Hash));
            }

            var entryStats = String.Format(
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

            result.AddRange(new[]
                {
                    new ListItem("Filesize", logFiles.Sum(l => l.Filesize) + " byte(s)"),
                    new ListItem("Full Filename", singleView ? logFile.FullFilename : null)
                        {
                            Details = GetFullFilenamePropertyDetail(logFiles),
                            Readonly = !singleView
                        },
                    new ListItem("Session Format", singleView ? logFile.SessionFormat : null)
                        {
                            Details = GetSessionFormatPropertyDetail(logFiles),
                            Readonly = !singleView || logFile.SessionFormat != null,
                            ComboDataSource = _session.Formats.ToList(),
                            ComboDisplayMember = "Name"
                        },
                    new ListItem("Hashing Status", singleView ? getEngineStatusDropdown(logFile.HashStatus) : null)
                        {
                            Details = singleView ? null : GetAggregateProperty(v => v.HashStatus, n => n.ToString()),
                            ComboDataSource = _engineStatusProperties,
                            ComboDisplayMember = "Name",
                            Readonly = false
                        },
                    new ListItem("Filename Parsing Status", singleView ?getEngineStatusDropdown(logFile.FilenameParseStatus) : null)
                        {
                            Details = singleView ? null : GetAggregateProperty(v => v.FilenameParseStatus, n => n.ToString()),
                            ComboDataSource = _engineStatusProperties,
                            ComboDisplayMember = "Name",
                            Readonly = false
                        },
                    new ListItem("Entry Parsing Status", singleView ? getEngineStatusDropdown(logFile.EntryParseStatus) : null)
                        {
                            Details = singleView ? null : GetAggregateProperty(v => v.EntryParseStatus, n => n.ToString()),
                            ComboDataSource = _engineStatusProperties,
                            ComboDisplayMember = "Name",
                            Readonly = false
                        },
                    new ListItem("Entry Statistics")
                        {
                            Details = entryStats
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
            var singleView = logFiles.Length == 1;
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

                    result.Add(new ListItem(sessionField.Display)
                    {
                        Data = sessionField,
                        Value = singleView ? _session.FieldValues.Find(logFile, formatField) : null,
                        Details = GetSessionFieldText(sessionField, GetPadding(2)),
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
            var singleView = logFiles.Length == 1;
            var result = new Dictionary<SessionFormat, List<ListItem>>();

            // Determine all Session and Format Fields within the LogFiles
            foreach (var l in logFiles) {
                var sessionFormat = l.SessionFormat;

                // If SessionFormat is not known, the fields are not known either 
                if (sessionFormat == null)
                    continue;

                // Only FormatFields within the Filename context are relevant
                var items = sessionFormat.Contexts[FormatContextEnum.Filename].Fields.ToList();
                List<ListItem> formatFields;

                if (!result.TryGetValue(sessionFormat, out formatFields)) {
                    formatFields = new List<ListItem>();
                    result.Add(sessionFormat, formatFields);
                }

                foreach (var f in items) {
                    if (formatFields.Count > 0 && formatFields.Exists(x => ReferenceEquals(x.Data as FormatField, f)))
                        continue;

                    formatFields.Add(new ListItem(f.Display)
                    {
                        Data = f,
                        Value = singleView ? _session.FieldValues.Find(l, f) : null,
                        Details = GetFormatFieldText(f), /** TEMP **/
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
        private string GetAggregateProperty<T>(Func<LogFile, T> valueFunc, Func<T, string> nameFunc)  {
            if (_logFiles.Length == 1)
                return nameFunc(valueFunc(_logFiles[0]));

            var counts = new Dictionary<T, AggregateProperty>();
            var width = 10;

            for (var i = 0; i < _logFiles.Length; i++) {
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

    }
}