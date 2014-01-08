using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medidata.Lumberjack.Core;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Logging;
using Medidata.Lumberjack.Core.Processing;
using Medidata.Lumberjack.UI.Properties;
using ListViewItemCollection = System.Windows.Forms.ListView.ColumnHeaderCollection;

namespace Medidata.Lumberjack.UI
{
    public partial class MainForm : Form
    {
        #region Private fields

        private const string LogFilesListViewColumnsKey = "LogFilesListViewColumns";
        private const string EntriesListViewColumnsKey = "EntriesListViewColumns";
        private const int MaxMessageLines = 4096;

        private static readonly object _locker = new object();

        private readonly Logger _logger;
        private readonly UserSession _session;

        private readonly StringBuilder _messageBuffer = new StringBuilder();

        private DateTime _lastUpdate = DateTime.MinValue;

        #endregion

        #region Initializers

        /// <summary>
        /// Creates a new MainForm instance.
        /// </summary>
        public MainForm() {
            _logger = Program.Logger;
            _session = Program.UserSession;
     
            InitializeComponent();

            var sorter = new ListViewColumnSorter {OnCompare = listViewColumnSorter_OnCompare};
            logsListView.ListViewItemSorter = sorter;
        }

        #endregion

        #region Main form even handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_OnLoad(object sender, EventArgs e) {
            _logger.Trace("UI-MF-001");

            messageTimer.Enabled = true;

            _session.ProcessController.ProgressChanged += ProcessController_ProgressChanged;
            _session.ProcessController.LogCompleted += ProcessController_LogCompleted;
            _session.Message += UserSession_Message;
            _session.LoadConfig();

            ShowMessages((bool)Settings.Default["ShowMessages"]);
            ShowLogFiles((bool)Settings.Default["ShowLogFiles"]);
            ShowEntries((bool)Settings.Default["ShowEntries"]);


            LoadListViewColumns(LogFilesListViewColumnsKey, logsListView, FieldContextFlags.Filename);
            LoadListViewColumns(EntriesListViewColumnsKey, entriesListView, FieldContextFlags.Entry | FieldContextFlags.Content);

            _logger.Trace("UI-MF-009");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            _session.ProcessController.ProgressChanged -= ProcessController_ProgressChanged;
            _session.ProcessController.LogCompleted -= ProcessController_LogCompleted;
            _session.Message -= UserSession_Message;

            _session.ProcessController.Stop();

            SaveListViewColumns(LogFilesListViewColumnsKey, logsListView);
            SaveListViewColumns(EntriesListViewColumnsKey, entriesListView);
            messageTimer.Enabled = false;
        }

        #endregion

        #region Menu item event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newSessionToolStripMenuItem_Click(object sender, EventArgs e) {
            // TODO: Implement
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openSessionToolStripMenuItem_Click(object sender, EventArgs e) {
            // TODO: Implement
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveSessionToolStripMenuItem_Click(object sender, EventArgs e) {
            // TODO: Implement
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveSessionAsToolStripMenuItem_Click(object sender, EventArgs e) {
            // TODO: Implement
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sessionPropertiesToolStripMenuItem_Click(object sender, EventArgs e) {
            // TODO: Implement
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addLocalLogsToolStripMenuItem_Click(object sender, EventArgs e) {
            // TODO: Implement
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addLocalLogsContextToolStripMenuItem_Click(object sender, EventArgs e) {
            addLocalLogsToolStripMenuItem.PerformClick();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addLogsFromSFTPToolStripMenuItem_Click(object sender, EventArgs e) {
            // TODO: Implement
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addLogsFromSFTPContextToolStripMenuItem_Click(object sender, EventArgs e) {
            addLogsFromSFTPToolStripMenuItem.PerformClick();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeSelectedLogsToolStripMenuItem_Click(object sender, EventArgs e) {
            RemoveLogFiles(logsListView, logsListView.SelectedItems);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeSelectedContextToolStripMenuItem_Click(object sender, EventArgs e) {
            removeSelectedLogsToolStripMenuItem.PerformClick();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearLogsToolStripMenuItem_Click(object sender, EventArgs e) {
            RemoveLogFiles(logsListView, logsListView.Items);

            logsListView.SelectedItems.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearLogsContextToolStripMenuItem_Click(object sender, EventArgs e) {
            clearLogsToolStripMenuItem.PerformClick();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logPropertiesToolStripMenuItem_Click(object sender, EventArgs e) {
            var items = logsListView.SelectedItems;
            if (items.Count == 0)
                return;

            var logs = (from ListViewItem lvi in items select ((LogFile)lvi.Tag)).ToArray();
            (new LogPropertiesForm(_session, logs)).ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logPropertiesContextToolStripMenuItem_Click(object sender, EventArgs e) {
            logPropertiesToolStripMenuItem.PerformClick();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e) {
            (new OptionsForm()).ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logFilesToolStripMenuItem_Click(object sender, EventArgs e) {
            var check = !logFilesToolStripMenuItem.Checked;

            ShowLogFiles(check);
            Settings.Default["ShowLogFiles"] = check;
            Settings.Default.Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logEntriesToolStripMenuItem_Click(object sender, EventArgs e) {
            var check = !logEntriesToolStripMenuItem.Checked;

            ShowEntries(check);
            Settings.Default["ShowEntries"] = check;
            Settings.Default.Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void messagesToolStripMenuItem_Click(object sender, EventArgs e) {
            var check = !messagesToolStripMenuItem.Checked;

            ShowMessages(check);
            Settings.Default["ShowMessages"] = check;
            Settings.Default.Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void filtersToolStripMenuItem_Click(object sender, EventArgs e) {
            (new FiltersForm()).ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void downloadQueueToolStripMenuItem_Click(object sender, EventArgs e) {
            (new DownloadQueueForm()).ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logCacheToolStripMenuItem_Click(object sender, EventArgs e) {
            (new LogCacheForm()).ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fieldEditorToolStripMenuItem_Click(object sender, EventArgs e) {
            (new FieldEditorForm()).ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formatEditorToolStripMenuItem_Click(object sender, EventArgs e) {
            (new FormatEditorForm()).ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            (new AboutForm()).ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectColumnsToolStripMenuItem_Click(object sender, EventArgs e) {
            var selectColumnsForm = new SelectColumnsForm(_session, LogFilesListViewColumnsKey, EntriesListViewColumnsKey);

            selectColumnsForm.ShowDialog();

            var cols = selectColumnsForm.LogFileColumns;
            if (cols != null) {
                SetListViewColumns(LogFilesListViewColumnsKey, logsListView, cols.ToArray(), FieldContextFlags.Filename);
            }

            cols = selectColumnsForm.LogEntryColumns;
            if (cols != null) {
                SetListViewColumns(EntriesListViewColumnsKey, entriesListView, cols.ToArray(), FieldContextFlags.Entry | FieldContextFlags.Content);
            }
        }

        #endregion

        #region Control event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logsListView_DragEnter(object sender, DragEventArgs e) {
            //if (_mergeEngine.IsRunning)
            //    e.Effect = DragDropEffects.None;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logsListView_DragDrop(object sender, DragEventArgs e) {
            //if (_mergeEngine.IsRunning)
            //    return;

            AddLogsToSession((string[])e.Data.GetData(DataFormats.FileDrop));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logsListView_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode != Keys.Delete)
                return;

            var control = (ListView) sender;
            RemoveLogFiles(control, control.SelectedItems);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logsListView_ColumnClick(object sender, ColumnClickEventArgs e) {
            var control = (ListView) sender;
            var sorter = (ListViewColumnSorter)control.ListViewItemSorter;

            if (e.Column == sorter.SortColumn) {
                if (sorter.Order == SortOrder.Ascending) {
                    sorter.Order = SortOrder.Descending;
                } else {
                    sorter.Order = SortOrder.Ascending;
                }
            } else {
                sorter.SortColumn = e.Column;
                sorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            control.Sort();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logsListView_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button != MouseButtons.Right)
                return;

            logsContextMenuStrip.Show(logsListView, e.Location);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
            var control = (ListView)sender;
            var selections = control.SelectedItems.Count > 0;

            removeSelectedLogsToolStripMenuItem.Enabled = selections;
            removeSelectedContextToolStripMenuItem.Enabled = selections;
            logPropertiesToolStripMenuItem.Enabled = selections;
            logPropertiesContextToolStripMenuItem.Enabled = selections;
        }
        
        #endregion

        #region Non-UI event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void UserSession_Message(object source, MessageEventArgs e) {
            this.Invoke(() =>
                {
                    lock (_locker) {
                        _messageBuffer.AppendFormat("[{0:yyyy-MM-dd HH:mm:ss}]  {1}\r\n", e.Timestamp, e.Message);
                    }

                }, false);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void messageTimer_Tick(object sender, EventArgs e) {
            string text = "";
            int length;

            lock (_locker) {
                length = _messageBuffer.Length;
                if (length > 0) {
                    text = _messageBuffer.ToString();
                    _messageBuffer.Clear();
                }
            }

            if (length == 0 || string.IsNullOrEmpty(text))
                return;

            messagesTextBox.Text += text;

            if (messagesTextBox.Lines.Length > MaxMessageLines) {
                messagesTextBox.ClearTopLines(MaxMessageLines - 100);
            }

            messagesTextBox.SelectionStart = messagesTextBox.Text.Length;
            messagesTextBox.ScrollToCaret();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void ProcessController_LogCompleted(object source, ProcessEventArgs e) {
            var logFile = e.LogFile;

            if (e.ProcessType == ProcessTypeEnum.Hash && logFile.HashStatus == EngineStatusEnum.Completed) {
                var dupe = _session.LogFiles.Any(x => !ReferenceEquals(x, logFile) && x.Md5Hash != null && x.Md5Hash.Equals(logFile.Md5Hash));
                if (dupe) {
                    var msg = String.Format("File with hash {0} already exists in session. Removing \"{1}\".", logFile.Md5Hash, logFile.Filename);

                    _session.InvokeMessage(this, new MessageEventArgs(MessageTypeEnum.Status, msg));
                    _session.LogFiles.Remove(logFile);

                    this.Invoke(() => {
                        for (var i = 0; i < logsListView.Items.Count; i++) {
                            if (e.LogFile != logsListView.Items[i].Tag)
                                continue;

                            logsListView.Items.Remove(logsListView.Items[i]);
                            break;
                        }
                    }, false);

                    return;
                }
            }

            this.Invoke(() =>
                {
                    RefreshLogListItem(logsListView, e.LogFile);

                    var metrics = e.EngineMetrics;
                    if (metrics.ProcessedBytes !=metrics.TotalBytes) {
                        var x = e;
                    }

                    mainStripStatusLabel.Text = String.Format("{4}: Completed {0} of {1} logs ({2} of {3} bytes)",
                                                              metrics.ProcessedLogs,
                                                              metrics.TotalLogs,
                                                              metrics.ProcessedBytes,
                                                              metrics.TotalBytes,
                                                              e.ProcessType);
                }, false);
        }
        
        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void ProcessController_ProgressChanged(object source, ProcessEventArgs e) {
            if (DateTime.UtcNow.Subtract(_lastUpdate).Seconds < 1) return;

            this.Invoke(() =>
                {
                    RefreshLogListItem(logsListView, e.LogFile);

                    var metrics = e.EngineMetrics;
                    mainStripStatusLabel.Text = String.Format("{5}: Processed {0} of {1} logs ({2} of {3} bytes) {4}%",
                                                              metrics.ProcessedLogs,
                                                              metrics.TotalLogs,
                                                              metrics.ProcessedBytes,
                                                              metrics.TotalBytes,
                                                              (float)((float)metrics.ProcessedBytes / (float)metrics.TotalBytes) * 100,
                                                              e.ProcessType);
                }, false);
            _lastUpdate = DateTime.UtcNow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="colIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static int listViewColumnSorter_OnCompare(ListViewColumnSorter sender, int colIndex, ListViewItem x, ListViewItem y) {
            var comparer = sender.ObjectCompare;
            var textX = x.SubItems[colIndex].Text;
            var textY = y.SubItems[colIndex].Text;
            var colText = x.ListView.Columns[colIndex].Text;
            var field = (SessionField)x.ListView.Columns[colIndex].Tag;

            var intX = 0;
            var intY = 0;

            // Column represents an internal (non-field) value
            if (field == null) {
                var logFileX = (LogFile)x.Tag;
                var logFileY = (LogFile)y.Tag;
                long longX;
                long longY;

                switch (colText) {
                    case "Size":
                        longX = logFileX.Filesize;
                        longY = logFileY.Filesize;

                        return comparer.Compare(longX, longY);
                    case "Total Entries":
                        longX = logFileX.EntryStats.TotalEntries;
                        longY = logFileY.EntryStats.TotalEntries;

                        return comparer.Compare(longX, longY);
                    case "DEBUG":
                    case "INFO":
                    case "WARN":
                    case "ERROR":
                    case "TRACE":
                    case "FATAL":
                        Int32.TryParse(textX, out intX);
                        Int32.TryParse(textY, out intY);

                        return comparer.Compare(intX, intY);

                    default:
                        return comparer.Compare(textX, textY);
                }
            }

            // Column represents a field value
            if (field.DataType == FieldDataTypeEnum.Integer) {
                field.TryUnformatValue(textX, ref intX);
                field.TryUnformatValue(textY, ref intY);

                return comparer.Compare(intX, intY);
            }

            if (field.DataType == FieldDataTypeEnum.DateTime) {
                var dateX = new DateTime();
                var dateY = new DateTime();

                field.TryUnformatValue(textX, ref dateX);
                field.TryUnformatValue(textY, ref dateY);

                return comparer.Compare(dateX, dateY);
            }

            return comparer.Compare(textX, textY);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visible"></param>
        private void ShowEntries(bool visible) {
            logEntriesToolStripMenuItem.Checked = visible;
            mainSplitContainer.Panel2Collapsed = !visible;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visible"></param>
        private void ShowLogFiles(bool visible) {
            logFilesToolStripMenuItem.Checked = visible;
            mainSplitContainer.Panel1Collapsed = !visible;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visible"></param>
        private void ShowMessages(bool visible) {
            messagesToolStripMenuItem.Checked = visible;
            messagesPanel.Visible = visible;

            var height = messagesPanel.Height + messagesPanel.Margin.Top + messagesPanel.Margin.Bottom;
            mainSplitContainer.Height += visible ? -height : height;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="lvItemCollection"></param>
        private void RemoveLogFiles<T>(ListView control, T lvItemCollection) where T : IEnumerable, IList, ICollection {
            //if (_logJoiner.IsRunning)
            //    return;

            if (lvItemCollection.Count == 0)
                return;

            bool isEmpty;

            lock (_locker) {
                var items = (from ListViewItem lvi in lvItemCollection select lvi).ToArray();
                var logFiles = (from ListViewItem lvi in items select (LogFile) lvi.Tag).ToArray();

                _session.LogFiles.Remove(logFiles);

                // Remove the items from the list view control
                for (var i = 0; i < items.Length; i++)
                    control.Items.Remove(items[i]);

                control.SelectedItems.Clear();
                isEmpty = control.Items.Count == 0;
            }

            clearLogsToolStripMenuItem.Enabled = !isEmpty;
            clearLogsContextToolStripMenuItem.Enabled = !isEmpty;
        }

        /// <summary>
        /// Adds the specified log files to the session and updates the list view
        /// control with the new items. The session should internally handle parsing
        /// of the log files automatically.
        /// </summary>
        /// <param name="filenames">Array of filenames of the log files to add.</param>
        private void AddLogsToSession(string[] filenames) {
            var lvis = new List<ListViewItem>(filenames.Length);
            ListViewItemCollection lvic;

            lock (_locker)
                lvic = logsListView.Columns;

            // Add the log files to the session
            _session.LogFiles.Add(filenames);

            try {
                // Create list view items for the log files and add to the control
                foreach (var filename in filenames) {
                    var items = new string[lvic.Count];

                    var logFile = _session.LogFiles.Find(filename);
                    foreach (ColumnHeader c in lvic) {
                        items[c.DisplayIndex] = GetListViewColumnText(c, logFile);
                    }

                    // Set the Tag property of each item to the LogFile it represents
                    lvis.Add(new ListViewItem(items) { Tag = logFile });
                }
            } catch (Exception ex) {
                var y = ex;
            }

            lock (_locker)
                logsListView.Items.AddRange(lvis.ToArray());

            var enabled = lvis.Count > 0;
            clearLogsToolStripMenuItem.Enabled = enabled;
            clearLogsContextToolStripMenuItem.Enabled = enabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="item"></param>
        private void RefreshLogListItem(ListView listView, ListViewItem item) {
            _logger.Debug("Updating item: " + item.Text);

            try {
                var logFile = (LogFile) item.Tag;
                var items = new string[listView.Columns.Count - 1];

                item.SubItems.Clear();

                foreach (ColumnHeader c in listView.Columns) {
                    var value = GetListViewColumnText(c, logFile);

                    if (c.DisplayIndex == 0) {
                        item.Text = value;
                    } else {
                        items[c.DisplayIndex - 1] = value;
                    }
                }

                item.SubItems.AddRange(items);
            } catch (Exception ex) {
                _logger.Error("Failed to refresh Log list item", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="logFile"></param>
        private void RefreshLogListItem(ListView listView, LogFile logFile) {
            foreach (ListViewItem item in listView.Items) {
                if (item.Tag != logFile) continue;

                RefreshLogListItem(listView, item);
                break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        private string GetListViewColumnText(ColumnHeader col, LogFile log) {
            SessionField sessionField;
            DateTime dateValue;

            switch (col.Text) {
                case "Filename":
                    return log.Filename;
                case "Size":
                    return log.Filesize.ToString();
                case "Format":
                    return log.SessionFormat != null ? log.SessionFormat.Name : "(unknown)";
                case "MD5 Hash":
                    return log.Md5Hash;
                case "Total Entries":
                    return log.EntryStats.TotalEntries.ToString();
                case "DEBUG":
                    return log.EntryStats.Debug.ToString();
                case "INFO":
                    return log.EntryStats.Info.ToString();
                case "WARN":
                    return log.EntryStats.Warn.ToString();
                case "ERROR":
                    return log.EntryStats.Error.ToString();
                case "TRACE":
                    return log.EntryStats.Trace.ToString();
                case "FATAL":
                    return log.EntryStats.Fatal.ToString();
                case "First Entry":
                    dateValue = log.EntryStats.FirstEntry;
                    sessionField = (SessionField)col.Tag;
                    return dateValue != default(DateTime) ? sessionField.FormatValue(dateValue) : "(unknown)";

                case "Last Entry":
                    dateValue = log.EntryStats.LastEntry;
                    sessionField = (SessionField)col.Tag;
                    return dateValue != default(DateTime) ? sessionField.FormatValue(dateValue) : "(unknown)";

                default:
                    sessionField = (SessionField)col.Tag;
                    if (!sessionField.ContextFlags.HasFlag(FieldContextFlags.Filename)) 
                        throw new InvalidOperationException("Cannot retrieve value for field '" + sessionField.Name + "': does not exist within 'Filename' context.");
                    
                    // Get the field value and format it based on the info specified by Field
                    var formatField = _session.FormatFields.Find(sessionField.Name, FieldContextFlags.Filename);
                    var value = _session.FieldValues.Find(log, formatField) ?? "";

                    return value.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Settings key which contains the columns. Format is: [Text:Width[,...]]</param>
        /// <param name="control">List view control to load the columns for.</param>
        /// <param name="columns">Array of strings indicating each column's text value.</param>
        /// <param name="containerType">
        /// Enum indicating the container type of the fields which the list view can have columns for.
        /// </param>
        private void SetListViewColumns(string key, ListView control, string[] columns, FieldContextFlags containerType) {
            var savedCols = new Dictionary<string, Int32>();
            var newCols = new List<string>();
            string settingVal;

            if (columns.Length == 0) {
                settingVal = SettingsUtil.RestoreDefaultSetting(key);
            } else {
                settingVal = SettingsUtil.GetSettingOrDefault(key);
            }

            var pairs = settingVal.Split(',');

            // Create Dictionary<DisplayText, Width> of the current columns 
            foreach (var p in pairs) {
                var parts = p.Split(':');
                savedCols.Add(parts[0], Int32.Parse(parts[1]));
            }

            // Create dictionary of the new columns to use, preserving the order
            // and width of existing columns
            foreach (var c in columns) {
                Int32 width;

                if (savedCols.TryGetValue(c, out width)) {
                    newCols.Add(c + ":" + width);
                } else {
                    newCols.Add(c + ":" + 100);
                }
            }

            // Save the new list view columns and re-load
            SettingsUtil.SetSettingValue(key, String.Join(",", newCols), true);

            LoadListViewColumns(key, control, containerType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Settings key which contains the columns. Format is: [Text:Width[,...]]</param>
        /// <param name="control">List view control to save the columns for.</param>
        private static void SaveListViewColumns(string key, ListView control) {
            var values = new List<string>();

            foreach (ColumnHeader col in control.Columns) {
                values.Add(col.Text + ":" + col.Width);
            }

            SettingsUtil.SetSettingValue(key, String.Join(",", values), true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Settings key which contains the columns. Format is: [Text:Width[,...]]</param>
        /// <param name="control">List view control to load the columns for.</param>
        /// <param name="containerType">
        /// Enum indicating the container type of the fields which the list view can have columns for.
        /// </param>
        private void LoadListViewColumns(string key, ListView control, FieldContextFlags containerType) {
            var value = SettingsUtil.GetSettingOrDefault(key);
            var fields = _session.SessionFields.FindAll(containerType).ToArray();

            // Reset the current columns for the list view
            control.Columns.Clear();

            // Create each column of the specified width and set the Tag value
            var cols = value.Split(',');
            foreach (var col in cols) {
                var parts = col.Split(':');
                var text = parts[0];
                SessionField sessionField;

                // First and Last Entry are a special case since they refer to a value
                // which originated an entry's TIMESTAMP Field. For consistant formatting
                // the column will have the Tag set to the Field so it can be used elsewhere
                if (text.Equals("First Entry") || text.Equals("Last Entry")) {
                    sessionField = _session.SessionFields.Find("TIMESTAMP");
                } else {
                    sessionField = fields.FirstOrDefault(x => x.Display != null && x.Display.Equals(text));
                }

                // Add the column with Tag set to the field
                var colKey = sessionField == null ? text : sessionField.Name;
                control.Columns.Add(colKey, text);
                control.Columns[colKey].Tag = sessionField;
                control.Columns[colKey].Width = Int32.Parse(parts[1]);
            }

            // Refresh the text of each item in the list view
            foreach (ListViewItem item in control.Items) {
                RefreshLogListItem(control, item);
            }
        }

        #endregion


    }
}