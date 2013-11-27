using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Medidata.Lumberjack.Logging;
using Medidata.Lumberjack.Logging.Processors;

namespace Medidata.Lumberjack
{
    public partial class MainForm : Form
    {
        #region Private fields

        private static readonly object _locker = new object();
        private readonly Merger _logJoiner = Merger.Instance;
        private readonly ListViewColumnSorter _lvwColumnSorter;
        private static readonly Session _session = new Session();

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            _lvwColumnSorter = new ListViewColumnSorter();
            joinerLogsListView.ListViewItemSorter = _lvwColumnSorter;
        }

        #endregion

        #region Main form even handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            startDateTimePicker.Value = startDateTimePicker.MinDate;
            endDateTimePicker.Value = startDateTimePicker.MaxDate;

            // Load configurable data
            _session.FieldConfigurator.Load();
            _session.FormatConfigurator.Load();
            _session.NodeConfigurator.Load();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !Shutdown();
        }

        #endregion

        #region Control event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Shutdown())
                Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, EventArgs e)
        {
            if (_logJoiner.IsRunning)
            {
                startButton.Text = "Stopping";
                startButton.Enabled = false;
                _logJoiner.Stop();
                return;
            }

            if (!CheckAllLogsScanned())
            {
                MessageBox.Show("Cannot start joining the log files until all queued logs have been parsed.", "Cannot continue", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = saveLogFileDialog.ShowDialog();
            if (result != DialogResult.OK) return;

            startButton.Text = "Stop";
            pauseButton.Enabled = true;
            removeLogFileToolStripMenuItem.Enabled = false;
            addLogFileToolStripMenuItem.Enabled = false;

            var startTime = startDateTimePicker.Value;
            var endTime = endDateTimePicker.Value;

            _logJoiner.OnCompleted += logJoiner_OnCompleted;
            _logJoiner.OnStatusChange += logJoiner_OnStatusChange;
            _logJoiner.Start(_session, saveLogFileDialog.FileName, startTime, endTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pauseButton_Click(object sender, EventArgs e)
        {
            if (!_logJoiner.IsRunning) return;

            if (_logJoiner.IsPaused)
            {
                pauseButton.Text = "Pause";
                _logJoiner.Resume();
            }
            else
            {
                pauseButton.Text = "Resume";
                _logJoiner.Pause();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void joinerLogsListView_DragEnter(object sender, DragEventArgs e)
        {
            if (_logJoiner.IsRunning)
                e.Effect = DragDropEffects.None;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void joinerLogsListView_DragDrop(object sender, DragEventArgs e)
        {
            if (_logJoiner.IsRunning) return;

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            AddLogToList(files);

            if (logScannerWorker.IsBusy || files.Length <= 0) return;

            startButton.Enabled = false;
            logScannerWorker.RunWorkerAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void joinerLogsListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete) return;

            RemoveSelectedLogs();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void joinerLogsListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == _lvwColumnSorter.SortColumn)
            {
                if (_lvwColumnSorter.Order == SortOrder.Ascending)
                    _lvwColumnSorter.Order = SortOrder.Descending;
                else
                    _lvwColumnSorter.Order = SortOrder.Ascending;
            }
            else
            {
                _lvwColumnSorter.SortColumn = e.Column;
                _lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            joinerLogsListView.Sort();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void joinerLogsListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            var count = joinerLogsListView.SelectedItems.Count;
            logFileContextMenu.Items["removeLogFileToolStripMenuItem"].Enabled = count > 0;
            logFileContextMenu.Items["clearLogFilesToolStripMenuItem"].Enabled = count > 0;
            logFileContextMenu.Items["propertiesToolStripMenuItem"].Enabled = count > 0;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_logJoiner.IsRunning) return;

            var result = addLogFileDialog.ShowDialog();
            if (result != DialogResult.OK) return;

            var files = addLogFileDialog.FileNames;
            foreach (var file in files)
                AddLogToList(file);

            if (logScannerWorker.IsBusy || files.Length <= 0) return;


            startButton.Enabled = false;
            logScannerWorker.RunWorkerAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedLogs();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearLogFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to remove all logs?", "Confirm removal!", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Cancel) return;

            if (!Shutdown())
                return;

            var items = joinerLogsListView.Items;
            //if (items.Count == 0) return;

            //lock (_locker)
           // {
            for (var i = 0; i < items.Count; i++)
            {
                var parsedLog = (Log)items[i].Tag;
                if (parsedLog.Stream != null) parsedLog.Stream.Dispose();
                if (parsedLog.StreamW != null) parsedLog.StreamW.Dispose();
            }

            //  _joinerLogQueue.Clear();
            joinerLogsListView.Items.Clear();
            _session.ClearLogs();
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var items = joinerLogsListView.SelectedItems;
            if (items.Count == 0) return;

            /*
             * var logs = new List<Log>(items.Count);
            lock (_locker)
            {
                for (var i = 0; i < items.Count; i++)
                {
                    logs.Add((Log)((Log)items[i].Tag).Clone());
                }
            }*/

            var logs = _session.GetLogs();

            new PropertiesForm(logs).ShowDialog();
        }

        #endregion

        #region Background worker handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logScannerWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //var parsedLog = new Log();

            while (true)
            {
                //var index = -1;
                //int count;

                if (logScannerWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                // Find the next log to parse
                var log = _session.GetLogs().FirstOrDefault(l => !l.IsParsed && l.FormatType != null && l.FileSize > 0);
                if (log == null) break;

                /*
                    lock (_locker)
                    {
                        count = _joinerLogQueue.Count;
                        _logCount = count;

                        for (var i = 0; i < count; i++)
                            if (!_joinerLogQueue[i].IsParsed)
                            {
                                index = i;
                                _logIndex = index;
                                parsedLog = _joinerLogQueue[i];
                                if (parsedLog.LogType == LogType.None) {
                                    continue;
                                }
                                break;
                            }
                    }

                    if (index == -1) break;
                    */

                //if (File.Exists(parsedLog.FullFilename + Log.SavedStateFileExt))

                if (_session.IsLogParseCached(log))
                {
                    //parsedLog.Deserialize();
                    //parsedLog.IsParsed = true;
                    log.Deserialize();
                    log.IsParsed = true;

                    //_joinerLogQueue[_logIndex] = parsedLog;
                    var progress = new ScannerProgress
                        {
                            //Log = parsedLog,
                            //BytesRead = parsedLog.BytesParsed,
                            Log = log,
                            BytesRead = log.BytesParsed,
                            CurrentLogIndex = 0, //_logIndex,
                            TotalLogs = 0 //_logCount
                        };

                    //logScannerWorker.ReportProgress((int)(((double)_logIndex / (double)_logCount) * 100), progress);
                    logScannerWorker.ReportProgress(0, progress);
                }
                else
                {

                    _session.Parser.UseDirAsNodeName = useParentDirAsNodeToolStripMenuItem.Checked;
                    _session.Parser.OnStatusChange += scanner_OnStatusChange;
                    _session.Parser.OnCompleted += scanner_OnCompleted;

                    log.Md5Hash = log.ComputeHash();
                    var success = _session.Parser.Parse(log);
                    if (!success)
                    {
                        log.FormatType = null; //TEMP
                    }
                    _session.Parser.OnCompleted -= scanner_OnCompleted;
                    _session.Parser.OnStatusChange -= scanner_OnStatusChange;
                    /*
                        var scanner = new Parser();

                        scanner.UseDirAsNodeName = useParentDirAsNodeToolStripMenuItem.Checked;
                        scanner.OnStatusChange += scanner_OnStatusChange;
                        scanner.OnCompleted += scanner_OnCompleted;

                        parsedLog.Md5Hash = parsedLog.ComputeHash();
                        var success = scanner.Parse(parsedLog);

                        scanner.OnCompleted -= scanner_OnCompleted;
                        scanner.OnStatusChange -= scanner_OnStatusChange;*/
                }
            }
        }

        void scanner_OnCompleted(object source, ParserEventArgs e)
        {
            //lock (_locker)
           // {
                //_joinerLogQueue[_logIndex] = e.CurParsedLog;

                var progress = new ScannerProgress
                {
                    Log = e.CurParsedLog,
                    BytesRead = e.CurParsedLog.BytesParsed,
                    CurrentLogIndex = 0,//_logIndex,
                    TotalLogs = 0//_logCount
                };

                //logScannerWorker.ReportProgress((int)(((double)_logIndex / (double)_logCount) * 100), progress);
                logScannerWorker.ReportProgress(0, progress);

                _session.CacheLogParse(e.CurParsedLog);
                //e.CurParsedLog.Serialize();

            //}
        }

        void scanner_OnStatusChange(object source, ParserEventArgs e)
        {
            //lock (_locker)
           // {
                var progress = new ScannerProgress
                {
                    Log = e.CurParsedLog,
                    BytesRead = e.CurParsedLog.BytesParsed,
                    CurrentLogIndex = 0,//_logIndex,
                    TotalLogs = 0//_logCount
                };

                //logScannerWorker.ReportProgress((int)(((double)_logIndex / (double)_logCount) * 100), progress);
                logScannerWorker.ReportProgress(0, progress);
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logScannerWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var progress = (ScannerProgress)e.UserState;
            var log = progress.Log;
            var percent = (((double)(progress.BytesRead) / (double)log.FileSize) * 100).ToString("##0.00");

            foreach (ListViewItem item in joinerLogsListView.Items)
            {
                var currentLog = (Log)item.Tag;
                if (currentLog.Filename != log.Filename) continue;
                if (currentLog.FullFilename != log.FullFilename) continue;
                //if (log.LogType == LogType.None) continue;
                //System.Diagnostics.Debug.WriteLine(currentLog.FullFilename);
                var entries = log.Entries;
                if (entries == null) continue;

                item.SubItems[3].Text = entries.Length.ToString(CultureInfo.InvariantCulture);
                item.SubItems[4].Text = entries.Length == 0 ? "" : log.StartTime.ToString("yyyy-MM-dd  HH:mm:ss.fff");
                item.SubItems[5].Text = entries.Length == 0 ? "" : log.EndTime.ToString("yyyy-MM-dd  HH:mm:ss.fff");
                item.SubItems[6].Text = entries.Length == 0 ? "" : log.DebugCount.ToString();
                item.SubItems[7].Text = entries.Length == 0 ? "" : log.InfoCount.ToString();
                item.SubItems[8].Text = entries.Length == 0 ? "" : log.WarnCount.ToString();
                item.SubItems[9].Text = entries.Length == 0 ? "" : log.ErrorCount.ToString();
                item.SubItems[10].Text = entries.Length == 0 ? "" : log.TraceCount.ToString();
            }

            if (!log.IsParsed)
            {
                mainStatusStrip.Items["mainStatusMessage"].Text = log.Filename + " (" + progress.BytesRead + " of " + log.FileSize + " bytes read) [" + percent + "%]";
            }
            else
            {
                mainStatusStrip.Items["mainStatusMessage"].Text = "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logScannerWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            startButton.Enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void logJoiner_OnCompleted(object source, MergerEventArgs e)
        {
            this.Invoke(() =>
            {
                startButton.Enabled = true;
                startButton.Text = "Start";
                pauseButton.Enabled = false;
                pauseButton.Text = "Pause";

                MessageBox.Show("Completed joining log files! Result: " + e.Action.ToString());

                mainStatusMessage.Text = "Completed!";

            }, false);


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void logJoiner_OnStatusChange(object source, MergerEventArgs e)
        {
            this.Invoke(() =>
            {
                mainStatusMessage.Text = e.Message;
            }, false);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool Shutdown()
        {
            var close = true;

            if (_logJoiner.IsRunning)
            {
                var start = DateTime.Now;

                _logJoiner.Stop();
                while (_logJoiner.IsRunning)
                {
                    Thread.Sleep(1000);
                    if (!(DateTime.Now.Subtract(start).TotalSeconds > 5)) continue;

                    var result = MessageBox.Show("Unable to stop the log joining thread. Do you want to keep waiting?", "Unable to stop thread!", MessageBoxButtons.YesNoCancel,
                                                 MessageBoxIcon.Exclamation);
                    if (result == DialogResult.No) break;
                    if (result == DialogResult.Cancel)
                    {
                        close = false;
                        break;
                    }

                    start = DateTime.Now;
                }
            }

            if (logScannerWorker.IsBusy)
            {
                logScannerWorker.CancelAsync();
                // Not sure if this is sufficient
            }

            return close;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RemoveSelectedLogs()
        {
            if (_logJoiner.IsRunning) return;

            var items = joinerLogsListView.SelectedItems;
            if (items.Count == 0) return;

            var logs = (from ListViewItem lvi in items select (Log) lvi.Tag).ToList();
            _session.RemoveLogRange(logs);

            for (var i = 0; i < items.Count; i++)
                joinerLogsListView.Items.Remove(items[i]);

            /*
            lock (_locker)
            {
                for (var i = 0; i < items.Count; i++)
                {
                    var parsedLog = (Log)items[i].Tag;
                    joinerLogsListView.Items.Remove(items[i]);

                    _joinerLogQueue.Remove(parsedLog);
                }
            }*/
        }
        
        private void AddLogToList(string[] filenames)
        {
            var lvis = new List<ListViewItem>(filenames.Length);
            var files = new List<string>(filenames.Length);

            files.AddRange(filenames.Select(t => t));
            var added = _session.AddLogRange(files);

            foreach (var l in added)
            {
                var items = new[]
                    {
                        l.Filename,
                        l.FormatType,
                        FormatFileSize(l.FileSize),
                        "?", "?", "?", "?", "?", "?", "?", "?"
                    };

                lvis.Add(new ListViewItem(items) {Tag = l});
            }
            joinerLogsListView.Items.AddRange(lvis.ToArray());

            /*
            var lvis = new List<ListViewItem>(filenames.Length);
            
            foreach (var t in filenames)
                if (_joinerLogQueue.All(item => item.FullFilename.ToUpper() != t.ToUpper()))
                {
                    var log = new Log(t);

                    var items = new[]
                        {
                            log.Filename,
                            log.LogType.ToString(),
                            FormatFileSize(log.FileSize),
                            "?", "?", "?", "?", "?", "?", "?", "?"
                        };

                    var item = new ListViewItem(items);

                    item.Tag = log;

                    lvis.Add(item);
                }

            lock (_locker)
            {
                joinerLogsListView.Items.AddRange(lvis.ToArray());
                var logs = new List<Log>(lvis.Count);

                logs.AddRange(lvis.Select(t => (Log) t.Tag));
                _joinerLogQueue.AddRange(logs);
            }*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        private void AddLogToList(string filename)
        {
            var log = _session.AddLog(filename);
            if (log == null) return;

            var items = new[]
                {
                    log.Filename,
                    log.FormatType,
                    FormatFileSize(log.FileSize),
                    "?", "?", "?", "?", "?", "?", "?", "?"
                };

            joinerLogsListView.Items.Add(new ListViewItem(items) {Tag = log});
            /*
            // Verify the file dose not already exist
            if (_joinerLogQueue.Any(item => item.FullFilename.ToUpper() == filename.ToUpper()))
                return;

            var parsedLog = new Log(filename);

            var items = new[]
                {
                    parsedLog.Filename,
                    parsedLog.LogType.ToString(),
                    FormatFileSize(parsedLog.FileSize),
                    "?", "?", "?", "?", "?", "?", "?", "?"
                };

            lock (_locker)
            {
                var item = new ListViewItem(items);
                item.Tag = parsedLog;
                joinerLogsListView.Items.Add(item);
                _joinerLogQueue.Add(parsedLog);
            }*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string FormatFileSize(long length)
        {
            if (length > 1024 * 1024 * 1024)
                return (length / (1024F * 1024 * 1024)).ToString("###.##") + " Gb";
            if (length > 1024 * 1024)
                return (length / (1024F * 1024)).ToString("###.##") + " Mb";
            if (length > 1024)
                return (length / 1024F).ToString("###.##") + " Kb";

            return length.ToString("###,###,##0") + " bytes";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CheckAllLogsScanned()
        {
            var parseComplete = true;

            if (_logJoiner.IsRunning)
                throw new InvalidOperationException();

            var logs = _session.GetLogs();
            lock (_locker)
            {
                for (var i = 0; i < logs.Count && parseComplete; i++)
                    parseComplete &= logs[i].IsParsed;
            }

            return parseComplete;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reportStartButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reportPauseButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void useParentDirAsNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            useParentDirAsNodeToolStripMenuItem.Checked = !useParentDirAsNodeToolStripMenuItem.Checked;
        }

    }

    public class ListViewColumnSorter : IComparer
    {
        /// <summary>
        /// Specifies the column to be sorted
        /// </summary>
        private int _columnToSort;

        /// <summary>
        /// Specifies the order in which to sort (i.e. 'Ascending').
        /// </summary>
        private SortOrder _orderOfSort;

        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        private readonly CaseInsensitiveComparer ObjectCompare;

        /// <summary>
        /// Class constructor.  Initializes various elements
        /// </summary>
        public ListViewColumnSorter()
        {
            _columnToSort = 0;
            _orderOfSort = SortOrder.None;
            ObjectCompare = new CaseInsensitiveComparer();
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn
        {
            set { _columnToSort = value; }
            get { return _columnToSort; }
        }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order
        {
            set { _orderOfSort = value; }
            get { return _orderOfSort; }
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            var listviewX = (ListViewItem)x;
            var listviewY = (ListViewItem)y;
            var textX = listviewX.SubItems[_columnToSort].Text;
            var textY = listviewY.SubItems[_columnToSort].Text;
            int compareResult;

            var colName = listviewX.ListView.Columns[_columnToSort].Text;
            switch (colName)
            {
                case "Entries":
                case "TRACE":
                case "DEBUG":
                case "INFO":
                case "WARN":
                case "ERROR":
                case "FATAL":
                    int intX;
                    int intY;

                    if (textX == "?") intX = int.MaxValue;
                    else int.TryParse(textX, out intX);

                    if (textY == "?") intY = int.MaxValue;
                    else int.TryParse(textY, out intY);

                    compareResult = ObjectCompare.Compare(intX, intY);
                    break;

                default:
                    compareResult = ObjectCompare.Compare(textX, textY);
                    break;
            }

            if (_orderOfSort == SortOrder.Ascending)
                return compareResult;

            if (_orderOfSort == SortOrder.Descending)
                return (-compareResult);

            return 0;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Execute a method on the control's owning thread.
        /// </summary>
        /// <param name="uiElement">The control that is being updated.</param>
        /// <param name="updater">The method that updates uiElement.</param>
        /// <param name="forceSynchronous">True to force synchronous execution of 
        /// updater.  False to allow asynchronous execution if the call is marshalled
        /// from a non-GUI thread.  If the method is called on the GUI thread,
        /// execution is always synchronous.</param>
        internal static void Invoke(this Control uiElement, Action updater, bool forceSynchronous)
        {
            if (uiElement == null)
                throw new ArgumentNullException();

            if (uiElement.InvokeRequired)
            {
                if (forceSynchronous)
                    uiElement.Invoke((Action)(() => Invoke(uiElement, updater, true)));
                else
                    uiElement.BeginInvoke((Action)(() => Invoke(uiElement, updater, false)));
            }
            else
            {
                if (!uiElement.IsHandleCreated)
                {
                    // Do nothing if the handle isn't created already.  The user's responsible
                    // for ensuring that the handle they give us exists.
                    return;
                }

                if (uiElement.IsDisposed)
                    throw new ObjectDisposedException("Control is already disposed.");

                updater();
            }
        }
    }
}
