using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medidata.Lumberjack.Core;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Data.Collections;
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

        private static readonly object _logLocker = new object();
        private static readonly object _entryLocker = new object();
        private static readonly object _locker = new object();

        private readonly Logger _logger;
        private readonly UserSession _session;

        private readonly StringBuilder _messageBuffer = new StringBuilder();

        private EntryView _entryView;
        private DateTime _lastUpdate = DateTime.MinValue;

        #endregion

        #region Initializers

        /// <summary>
        /// Creates a new MainForm instance.
        /// </summary>
        public MainForm() {
            _logger = Program.Logger;
            _session = Program.UserSession;
            _entryView = new EntryView(_session);

            InitializeComponent();

            var sorter = new ListViewColumnSorter { OnCompare = listViewColumnSorter_OnCompare };
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
            _session.FieldValues.ValueUpdated += FieldValues_ValueUpdated;
            _session.LogFiles.ItemUpdated += LogFiles_ItemUpdated;
            _session.LogFiles.ItemRemoved += LogFiles_ItemRemoved;
            _session.LogFiles.ItemAdded += LogFiles_ItemAdded;
            _session.Entries.ItemAdded += Entries_ItemAdded;
            _session.Entries.ItemUpdated += Entries_ItemUpdated;
            _session.ProcessController.ProgressChanged += ProcessController_ProgressChanged;
            _session.ProcessController.LogCompleted += ProcessController_LogCompleted;
            _session.Message += UserSession_Message;
            _session.LoadConfig();

            mainSplitContainer.SplitterDistance = Settings.Default.SplitterDistance;

            ShowMessages((bool)Settings.Default["ShowMessages"]);
            ShowLogFiles((bool)Settings.Default["ShowLogFiles"]);
            ShowEntries((bool)Settings.Default["ShowEntries"]);

            var columns = SettingsManager.LoadLogFilesColumns();
            LoadListViewColumns(columns, logsListView, FieldContextFlags.Filename);

            columns = SettingsManager.LoadEntriesColumns();
            LoadListViewColumns(columns, entriesListView, FieldContextFlags.Entry | FieldContextFlags.Content);

            _logger.Trace("UI-MF-009");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            _session.FieldValues.ValueUpdated -= FieldValues_ValueUpdated;
            _session.LogFiles.ItemUpdated -= LogFiles_ItemUpdated;
            _session.LogFiles.ItemRemoved -= LogFiles_ItemRemoved;
            _session.LogFiles.ItemAdded -= LogFiles_ItemAdded;
            _session.Entries.ItemAdded -= Entries_ItemAdded;
            _session.Entries.ItemUpdated -= Entries_ItemUpdated;
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
            RemoveLogFiles(logsListView.SelectedItems);
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
            RemoveLogFiles(logsListView.Items);

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
            var fields = (from ColumnHeader c in entriesListView.Columns select ((SessionField)c.Tag).Name).ToList();
            _entryView.RowFields = fields.ToArray();

            _entryView.Refresh();
            //(new AboutForm()).ShowDialog();
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
            _session.LogFiles.Add((string[])e.Data.GetData(DataFormats.FileDrop));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logsListView_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode != Keys.Delete)
                return;

            var control = (ListView)sender;
            RemoveLogFiles(control.SelectedItems);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logsListView_ColumnClick(object sender, ColumnClickEventArgs e) {
            var control = (ListView)sender;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainSplitContainer_SplitterMoved(object sender, SplitterEventArgs e) {
            if (!Visible)
                return;
            Settings.Default.SplitterDistance = e.SplitY;
            Settings.Default.Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void entryNavigatorToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            switch (e.ClickedItem.Name) {
                case "moveFirstItemToolStripButton":
                    break;
                case "movePreviousItemToolStripButton":
                    break;
                case "moveNextItemToolStripButton":
                    break;
                case "moveLastItemToolStripButton":
                    break;
            }
        }

        #endregion

        #region Non-UI event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void UserSession_Message(object source, MessageEventArgs e) {
            lock (_locker)
                _messageBuffer.AppendFormat("[{0:yyyy-MM-dd HH:mm:ss}]  {1}\r\n", e.Timestamp, e.Message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void FieldValues_ValueUpdated(object source, ValueUpdatedEventArgs e) {
            if (e.Component is LogFile)
                RefreshLogListItem(logsListView, e.Component as LogFile);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void LogFiles_ItemAdded(object source, CollectionItemEventArgs<LogFile> e) {
            var logFiles = e.Items;
            var lvis = new List<ListViewItem>(logFiles.Length);

            this.Invoke(() => {
                bool enabled;


                lock (_logLocker) {
                    var lvic = logsListView.Columns;

                    // Create list view items for the log files and add to the control
                    foreach (var logFile in logFiles) {
                        var items = new string[lvic.Count];

                        foreach (ColumnHeader c in lvic)
                            items[c.DisplayIndex] = GetListViewColumnText(c, logFile);

                        // Set the Tag property of each item to the LogFile it represents
                        lvis.Add(new ListViewItem(items) { Tag = logFile });
                    }

                    logsListView.Items.AddRange(lvis.ToArray());
                    enabled = lvis.Count > 0;
                }

                clearLogsToolStripMenuItem.Enabled = enabled;
                clearLogsContextToolStripMenuItem.Enabled = enabled;
            }, false);

            _session.ProcessController.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void LogFiles_ItemRemoved(object source, CollectionItemEventArgs<LogFile> e) {
            var logFiles = e.Items;

            this.Invoke(() => {
                bool isEmpty;

                lock (_logLocker) {
                    var items = logsListView.Items;

                    for (var i = 0; i < items.Count; i++) {
                        if (logFiles.Any(x => x.Id == ((LogFile)items[i].Tag).Id))
                            items.RemoveAt(i--);
                    }

                    isEmpty = items.Count == 0;
                }

                clearLogsToolStripMenuItem.Enabled = !isEmpty;
                clearLogsContextToolStripMenuItem.Enabled = !isEmpty;
            }, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void LogFiles_ItemUpdated(object source, CollectionItemEventArgs<LogFile> e) {
            e.Items.ToList().ForEach(x => RefreshLogListItem(logsListView, x));
            _session.ProcessController.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void Entries_ItemAdded(object source, CollectionItemEventArgs<Entry> e) {
            var entries = e.Items;
            var lvis = new List<ListViewItem>(entries.Length);
            var entryValues = new List<IFieldValue>[entries.Length];

            for (var i = 0; i < entries.Length; i++)
                entryValues[i] = _session.FieldValues.FindAll(entries[i]);

            this.Invoke(() => {
                lock (_entryLocker) {
                    var lvic = entriesListView.Columns;

                    // Create list view items for the log files and add to the control

                    for (var i = 0; i < entries.Length; i++) {
                        var entry = entries[i];
                        var items = new string[lvic.Count];

                        foreach (ColumnHeader c in lvic)
                            items[c.DisplayIndex] = (EntryFieldValue)entryValues[i].Find(x => x.FormatField.SessionField.Id == ((SessionField)c.Tag).Id);


                        // Set the Tag property of each item to the LogFile it represents
                        lvis.Add(new ListViewItem(items) { Tag = entry });
                    }

                    countItemToolStripButton.Text = "of " + _session.Entries.Count;
                    entriesListView.Items.AddRange(lvis.ToArray());
                }
            }, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void Entries_ItemUpdated(object source, CollectionItemEventArgs<Entry> e) {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void ProcessController_LogCompleted(object source, ProcessEventArgs e) {
            var logFile = e.LogFile;

            if (e.ProcessType == ProcessTypeEnum.Hash && logFile.HashStatus == EngineStatusEnum.Completed) {
                var dupe = _session.LogFiles.Any(x => x.Id != logFile.Id && x.Md5Hash != null && x.Md5Hash.Equals(logFile.Md5Hash));
                if (dupe) {
                    var msg = String.Format("File with hash {0} already exists in session. Removing \"{1}\".", logFile.Md5Hash, logFile.Filename);

                    _session.InvokeMessage(this, new MessageEventArgs(MessageTypeEnum.Status, msg));
                    _session.LogFiles.Remove(logFile);

                    return;
                }
            }

            this.Invoke(() => {
                    RefreshLogListItem(logsListView, e.LogFile);
                    mainStripStatusLabel.Text = GetStatusStripText(e.EngineMetrics, e.ProcessType, true);
                }, false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void ProcessController_ProgressChanged(object source, ProcessEventArgs e) {
            if (DateTime.UtcNow.Subtract(_lastUpdate).Seconds < 1)
                return;

            this.Invoke(() => {
                    RefreshLogListItem(logsListView, e.LogFile);
                    mainStripStatusLabel.Text = GetStatusStripText(e.EngineMetrics, e.ProcessType, false);
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

            Int32 intX;
            Int32 intY;

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
                field.TryUnformatValue(textX, out intX);
                field.TryUnformatValue(textY, out intY);

                return comparer.Compare(intX, intY);
            }

            if (field.DataType == FieldDataTypeEnum.DateTime) {
                DateTime dateX;
                DateTime dateY;

                field.TryUnformatValue(textX, out dateX);
                field.TryUnformatValue(textY, out dateY);

                return comparer.Compare(dateX, dateY);
            }

            return comparer.Compare(textX, textY);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void messageTimer_Tick(object sender, EventArgs e) {
            var text = "";
            int length;

            lock (_locker) {
                length = _messageBuffer.Length;
                if (length > 0) {
                    text = _messageBuffer.ToString();
                    _messageBuffer.Clear();
                }
            }

            if (length == 0 || String.IsNullOrEmpty(text))
                return;

            messagesTextBox.Text += text;

            if (messagesTextBox.Lines.Length > MaxMessageLines) {
                messagesTextBox.ClearTopLines(MaxMessageLines - 100);
            }

            messagesTextBox.SelectionStart = messagesTextBox.Text.Length;
            messagesTextBox.ScrollToCaret();
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
        /// <param name="lvItemCollection"></param>
        private void RemoveLogFiles<T>(T lvItemCollection) where T : IList, ICollection {
            LogFile[] logFiles;

            lock (_logLocker) {
                var len = lvItemCollection.Count;

                logFiles = new LogFile[len];
                for (var i = 0; i < len; i++)
                    logFiles[i] = ((LogFile)((ListViewItem)lvItemCollection[i]).Tag);
            }

            if (logFiles.Length > 0)
                _session.LogFiles.Remove(logFiles);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="item"></param>
        private void RefreshLogListItem(ListView listView, ListViewItem item) {
            _logger.Debug("Updating item: " + item.Text);

            try {
                var logFile = (LogFile)item.Tag;
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
                if (item.Tag != logFile)
                    continue;

                RefreshLogListItem(listView, item);
                break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="logFile"></param>
        /// <returns></returns>
        private string GetListViewColumnText(ColumnHeader col, LogFile logFile) {
            SessionField sessionField;
            DateTime dateValue;

            switch (col.Text) {
                case "Filename":
                    return logFile.Filename;
                case "Size":
                    return logFile.Filesize.ToString();
                case "Format":
                    return logFile.SessionFormat != null ? logFile.SessionFormat.Name : "(unknown)";
                case "MD5 Hash":
                    return logFile.Md5Hash;
                case "Total Entries":
                    return logFile.EntryStats.TotalEntries.ToString();
                case "DEBUG":
                    return logFile.EntryStats.Debug.ToString();
                case "INFO":
                    return logFile.EntryStats.Info.ToString();
                case "WARN":
                    return logFile.EntryStats.Warn.ToString();
                case "ERROR":
                    return logFile.EntryStats.Error.ToString();
                case "TRACE":
                    return logFile.EntryStats.Trace.ToString();
                case "FATAL":
                    return logFile.EntryStats.Fatal.ToString();
                case "First Entry":
                    dateValue = logFile.EntryStats.FirstEntry;
                    sessionField = (SessionField)col.Tag;
                    return dateValue != default(DateTime) ? sessionField.FormatValue(dateValue) : "(unknown)";

                case "Last Entry":
                    dateValue = logFile.EntryStats.LastEntry;
                    sessionField = (SessionField)col.Tag;
                    return dateValue != default(DateTime) ? sessionField.FormatValue(dateValue) : "(unknown)";

                default:
                    sessionField = (SessionField)col.Tag;
                    if (!sessionField.ContextFlags.HasFlag(FieldContextFlags.Filename))
                        throw new InvalidOperationException("Cannot retrieve value for field '" + sessionField.Name + "': does not exist within 'Filename' context.");

                    // Get the field value and format it based on the info specified by Field
                    var value = _session.FieldValues.Find(logFile, null, sessionField);
                    return value != null ? value.ToString() : "";
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Settings key which contains the columns. Format is: [Text:Width[,...]]</param>
        /// <param name="control">List view control to load the columns for.</param>
        /// <param name="columns">Array of strings indicating each column's text value.</param>
        /// <param name="contextFlags">
        /// Enum indicating the container type of the fields which the list view can have columns for.
        /// </param>
        [Obsolete]
        private void SetListViewColumns(string key, ListView control, string[] columns, FieldContextFlags contextFlags) {
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

            LoadListViewColumns(key, control, contextFlags);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Settings key which contains the columns. Format is: [Text:Width[,...]]</param>
        /// <param name="control">List view control to save the columns for.</param>
        [Obsolete]
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
        /// <param name="contextFlags">
        /// Enum indicating the container type of the fields which the list view can have columns for.
        /// </param>
        [Obsolete]
        private void LoadListViewColumns(string key, ListView control, FieldContextFlags contextFlags) {
            var value = SettingsUtil.GetSettingOrDefault(key);
            var fields = _session.SessionFields.FindAll(contextFlags).ToArray();

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="control"></param>
        /// <param name="contextFlags"></param>
        private void LoadListViewColumns(IEnumerable<ListViewColumn> columns, ListView control, FieldContextFlags contextFlags) {
            var fields = _session.SessionFields.FindAll(contextFlags).ToArray();

            // Reset the current columns for the list view
            control.Columns.Clear();

            // Create each column of the specified width and set the Tag value

            foreach (var col in columns) {
                SessionField sessionField;

                // First and Last Entry are a special case since they refer to a value
                // which originated an entry's TIMESTAMP Field. For consistant formatting
                // the column will have the Tag set to the Field so it can be used elsewhere
                if (col.Text.Equals("First Entry") || col.Text.Equals("Last Entry")) {
                    sessionField = _session.SessionFields.Find("TIMESTAMP");
                } else {
                    sessionField = fields.FirstOrDefault(x => x.Display != null && x.Display.Equals(col.Text));
                }

                // Add the column with Tag set to the field
                var colKey = sessionField == null ? col.Text : sessionField.Name;
                control.Columns.Add(colKey, col.Text);
                control.Columns[colKey].Tag = sessionField;
                control.Columns[colKey].Width = col.Width;
            }

            // Refresh the text of each item in the list view
            foreach (ListViewItem item in control.Items) 
                RefreshLogListItem(control, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metrics"></param>
        /// <param name="processType"></param>
        /// <param name="completed"></param>
        /// <returns></returns>
        private static string GetStatusStripText(EngineMetrics metrics, ProcessTypeEnum processType, bool completed) {
            return String.Format("{5}: {6} {0} of {1} logs ({2} of {3} bytes){4}",
                                 metrics.ProcessedLogs,
                                 metrics.TotalLogs,
                                 metrics.ProcessedBytes,
                                 metrics.TotalBytes,
                                 completed ? "" : (" " + ((float)((float)metrics.ProcessedBytes / (float)metrics.TotalBytes) * 100) + "%"),
                                 processType,
                                 completed ? "Completed" : "Processed");
        }

        #endregion

    }
}