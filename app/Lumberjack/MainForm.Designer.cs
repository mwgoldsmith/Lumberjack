namespace Medidata.Lumberjack
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.endDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.startDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pauseButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.joinerLogsListView = new System.Windows.Forms.ListView();
            this.filenameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.logletHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.filesizeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.countHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.firstDateHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lastDateHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.debugColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.infoColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.warnColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.errorColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.traceColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.logFileContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearLogFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useParentDirAsNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLogFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.logScannerWorker = new System.ComponentModel.BackgroundWorker();
            this.saveLogFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.mainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.mainStatusMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainStatusTotals = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.logFileContextMenu.SuspendLayout();
            this.mainMenuStrip.SuspendLayout();
            this.mainStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTabControl
            // 
            this.mainTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainTabControl.Controls.Add(this.tabPage1);
            this.mainTabControl.Location = new System.Drawing.Point(0, 24);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(988, 421);
            this.mainTabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.endDateTimePicker);
            this.tabPage1.Controls.Add(this.startDateTimePicker);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.pauseButton);
            this.tabPage1.Controls.Add(this.startButton);
            this.tabPage1.Controls.Add(this.joinerLogsListView);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(980, 395);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Aggregate Logs";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // endDateTimePicker
            // 
            this.endDateTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.endDateTimePicker.CustomFormat = "yyy-MM-dd HH:mm:ss";
            this.endDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endDateTimePicker.Location = new System.Drawing.Point(278, 372);
            this.endDateTimePicker.Name = "endDateTimePicker";
            this.endDateTimePicker.Size = new System.Drawing.Size(141, 20);
            this.endDateTimePicker.TabIndex = 6;
            this.endDateTimePicker.Value = new System.DateTime(2013, 3, 26, 19, 42, 23, 0);
            // 
            // startDateTimePicker
            // 
            this.startDateTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.startDateTimePicker.CustomFormat = "yyy-MM-dd HH:mm:ss";
            this.startDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startDateTimePicker.Location = new System.Drawing.Point(70, 372);
            this.startDateTimePicker.Name = "startDateTimePicker";
            this.startDateTimePicker.Size = new System.Drawing.Size(141, 20);
            this.startDateTimePicker.TabIndex = 5;
            this.startDateTimePicker.Value = new System.DateTime(2013, 5, 20, 0, 0, 0, 0);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(217, 374);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "End Time:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 374);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Start Time:";
            // 
            // pauseButton
            // 
            this.pauseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pauseButton.Enabled = false;
            this.pauseButton.Location = new System.Drawing.Point(821, 369);
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(75, 23);
            this.pauseButton.TabIndex = 2;
            this.pauseButton.Text = "Pause";
            this.pauseButton.UseVisualStyleBackColor = true;
            this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
            // 
            // startButton
            // 
            this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.startButton.Enabled = false;
            this.startButton.Location = new System.Drawing.Point(902, 369);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 1;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // joinerLogsListView
            // 
            this.joinerLogsListView.AllowColumnReorder = true;
            this.joinerLogsListView.AllowDrop = true;
            this.joinerLogsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.joinerLogsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.filenameHeader,
            this.logletHeader,
            this.filesizeHeader,
            this.countHeader,
            this.firstDateHeader,
            this.lastDateHeader,
            this.debugColumnHeader,
            this.infoColumnHeader,
            this.warnColumnHeader,
            this.errorColumnHeader,
            this.traceColumnHeader});
            this.joinerLogsListView.ContextMenuStrip = this.logFileContextMenu;
            this.joinerLogsListView.FullRowSelect = true;
            this.joinerLogsListView.GridLines = true;
            this.joinerLogsListView.Location = new System.Drawing.Point(3, 0);
            this.joinerLogsListView.Name = "joinerLogsListView";
            this.joinerLogsListView.ShowGroups = false;
            this.joinerLogsListView.Size = new System.Drawing.Size(974, 360);
            this.joinerLogsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.joinerLogsListView.TabIndex = 0;
            this.joinerLogsListView.UseCompatibleStateImageBehavior = false;
            this.joinerLogsListView.View = System.Windows.Forms.View.Details;
            this.joinerLogsListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.joinerLogsListView_ColumnClick);
            this.joinerLogsListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.joinerLogsListView_DragDrop);
            this.joinerLogsListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.joinerLogsListView_DragEnter);
            this.joinerLogsListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.joinerLogsListView_KeyDown);
            this.joinerLogsListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.joinerLogsListView_MouseClick);
            // 
            // filenameHeader
            // 
            this.filenameHeader.Text = "Filename";
            this.filenameHeader.Width = 220;
            // 
            // logletHeader
            // 
            this.logletHeader.Text = "Loglet";
            // 
            // filesizeHeader
            // 
            this.filesizeHeader.Text = "File Size";
            // 
            // countHeader
            // 
            this.countHeader.Text = "Entries";
            // 
            // firstDateHeader
            // 
            this.firstDateHeader.Text = "First Date";
            this.firstDateHeader.Width = 140;
            // 
            // lastDateHeader
            // 
            this.lastDateHeader.Text = "Last Date";
            this.lastDateHeader.Width = 140;
            // 
            // debugColumnHeader
            // 
            this.debugColumnHeader.Text = "DEBUG";
            // 
            // infoColumnHeader
            // 
            this.infoColumnHeader.Text = "INFO";
            // 
            // warnColumnHeader
            // 
            this.warnColumnHeader.Text = "WARN";
            // 
            // errorColumnHeader
            // 
            this.errorColumnHeader.Text = "ERROR";
            // 
            // traceColumnHeader
            // 
            this.traceColumnHeader.Text = "TRACE";
            // 
            // logFileContextMenu
            // 
            this.logFileContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLogFileToolStripMenuItem,
            this.removeLogFileToolStripMenuItem,
            this.clearLogFilesToolStripMenuItem,
            this.toolStripMenuItem1,
            this.propertiesToolStripMenuItem});
            this.logFileContextMenu.Name = "logFileContextMenu";
            this.logFileContextMenu.Size = new System.Drawing.Size(162, 98);
            // 
            // addLogFileToolStripMenuItem
            // 
            this.addLogFileToolStripMenuItem.Name = "addLogFileToolStripMenuItem";
            this.addLogFileToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.addLogFileToolStripMenuItem.Text = "Add Log File...";
            this.addLogFileToolStripMenuItem.Click += new System.EventHandler(this.addLogFileToolStripMenuItem_Click);
            // 
            // removeLogFileToolStripMenuItem
            // 
            this.removeLogFileToolStripMenuItem.Enabled = false;
            this.removeLogFileToolStripMenuItem.Name = "removeLogFileToolStripMenuItem";
            this.removeLogFileToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.removeLogFileToolStripMenuItem.Text = "Remove Log File";
            this.removeLogFileToolStripMenuItem.Click += new System.EventHandler(this.removeLogFileToolStripMenuItem_Click);
            // 
            // clearLogFilesToolStripMenuItem
            // 
            this.clearLogFilesToolStripMenuItem.Enabled = false;
            this.clearLogFilesToolStripMenuItem.Name = "clearLogFilesToolStripMenuItem";
            this.clearLogFilesToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.clearLogFilesToolStripMenuItem.Text = "Clear Log Files";
            this.clearLogFilesToolStripMenuItem.Click += new System.EventHandler(this.clearLogFilesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(158, 6);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Enabled = false;
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(988, 24);
            this.mainMenuStrip.TabIndex = 1;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.useParentDirAsNodeToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // useParentDirAsNodeToolStripMenuItem
            // 
            this.useParentDirAsNodeToolStripMenuItem.Checked = true;
            this.useParentDirAsNodeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useParentDirAsNodeToolStripMenuItem.Name = "useParentDirAsNodeToolStripMenuItem";
            this.useParentDirAsNodeToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.useParentDirAsNodeToolStripMenuItem.Text = "Use parent dir as node";
            this.useParentDirAsNodeToolStripMenuItem.Click += new System.EventHandler(this.useParentDirAsNodeToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // addLogFileDialog
            // 
            this.addLogFileDialog.Filter = "Log Files|*.log|All Files|*.*";
            this.addLogFileDialog.Multiselect = true;
            // 
            // logScannerWorker
            // 
            this.logScannerWorker.WorkerReportsProgress = true;
            this.logScannerWorker.WorkerSupportsCancellation = true;
            this.logScannerWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.logScannerWorker_DoWork);
            this.logScannerWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.logScannerWorker_ProgressChanged);
            this.logScannerWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.logScannerWorker_RunWorkerCompleted);
            // 
            // saveLogFileDialog
            // 
            this.saveLogFileDialog.AddExtension = false;
            this.saveLogFileDialog.Filter = "Log Files|*.log|All Files|*.*";
            this.saveLogFileDialog.Title = "Save log output as...";
            // 
            // mainStatusStrip
            // 
            this.mainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainStatusMessage,
            this.mainStatusTotals});
            this.mainStatusStrip.Location = new System.Drawing.Point(0, 448);
            this.mainStatusStrip.Name = "mainStatusStrip";
            this.mainStatusStrip.Size = new System.Drawing.Size(988, 22);
            this.mainStatusStrip.TabIndex = 2;
            // 
            // mainStatusMessage
            // 
            this.mainStatusMessage.Name = "mainStatusMessage";
            this.mainStatusMessage.Size = new System.Drawing.Size(0, 17);
            // 
            // mainStatusTotals
            // 
            this.mainStatusTotals.Name = "mainStatusTotals";
            this.mainStatusTotals.Size = new System.Drawing.Size(0, 17);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(988, 470);
            this.Controls.Add(this.mainStatusStrip);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.mainMenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "MainForm";
            this.Text = "CTMS Lumberjack";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.mainTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.logFileContextMenu.ResumeLayout(false);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.mainStatusStrip.ResumeLayout(false);
            this.mainStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DateTimePicker endDateTimePicker;
        private System.Windows.Forms.DateTimePicker startDateTimePicker;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button pauseButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.ListView joinerLogsListView;
        private System.Windows.Forms.ColumnHeader filenameHeader;
        private System.Windows.Forms.ColumnHeader logletHeader;
        private System.Windows.Forms.ColumnHeader filesizeHeader;
        private System.Windows.Forms.ColumnHeader countHeader;
        private System.Windows.Forms.ColumnHeader firstDateHeader;
        private System.Windows.Forms.ColumnHeader lastDateHeader;
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip logFileContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addLogFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeLogFileToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog addLogFileDialog;
        private System.ComponentModel.BackgroundWorker logScannerWorker;
        private System.Windows.Forms.SaveFileDialog saveLogFileDialog;
        private System.Windows.Forms.StatusStrip mainStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel mainStatusMessage;
        private System.Windows.Forms.ColumnHeader errorColumnHeader;
        private System.Windows.Forms.ColumnHeader warnColumnHeader;
        private System.Windows.Forms.ColumnHeader infoColumnHeader;
        private System.Windows.Forms.ColumnHeader traceColumnHeader;
        private System.Windows.Forms.ColumnHeader debugColumnHeader;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useParentDirAsNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel mainStatusTotals;
        private System.Windows.Forms.ToolStripMenuItem clearLogFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
    }
}

