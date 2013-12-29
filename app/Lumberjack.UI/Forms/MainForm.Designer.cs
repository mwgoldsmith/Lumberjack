namespace Medidata.Lumberjack.UI
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
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
            this.openSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.saveSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSessionAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.sessionPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLocalLogsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLogsFromSFTPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.removeSelectedLogsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearLogsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.logPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logEntriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.messagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripSeparator();
            this.filtersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadQueueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
            this.fieldEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripSeparator();
            this.selectColumnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.logsListView = new System.Windows.Forms.ListView();
            this.entriesListView = new System.Windows.Forms.ListView();
            this.messageTimer = new System.Windows.Forms.Timer(this.components);
            this.messagesPanel = new System.Windows.Forms.Panel();
            this.messagesTextBox = new System.Windows.Forms.TextBox();
            this.mainStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainMenuStrip.SuspendLayout();
            this.mainStatusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            this.messagesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(771, 24);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sessionToolStripMenuItem,
            this.logsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.optionsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // sessionToolStripMenuItem
            // 
            this.sessionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSessionToolStripMenuItem,
            this.toolStripMenuItem7,
            this.openSessionToolStripMenuItem,
            this.toolStripMenuItem6,
            this.saveSessionToolStripMenuItem,
            this.saveSessionAsToolStripMenuItem,
            this.toolStripMenuItem5,
            this.sessionPropertiesToolStripMenuItem});
            this.sessionToolStripMenuItem.Name = "sessionToolStripMenuItem";
            this.sessionToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.sessionToolStripMenuItem.Text = "Session";
            // 
            // newSessionToolStripMenuItem
            // 
            this.newSessionToolStripMenuItem.Name = "newSessionToolStripMenuItem";
            this.newSessionToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.newSessionToolStripMenuItem.Text = "&New";
            this.newSessionToolStripMenuItem.Click += new System.EventHandler(this.newSessionToolStripMenuItem_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(133, 6);
            // 
            // openSessionToolStripMenuItem
            // 
            this.openSessionToolStripMenuItem.Name = "openSessionToolStripMenuItem";
            this.openSessionToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.openSessionToolStripMenuItem.Text = "&Open...";
            this.openSessionToolStripMenuItem.Click += new System.EventHandler(this.openSessionToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(133, 6);
            // 
            // saveSessionToolStripMenuItem
            // 
            this.saveSessionToolStripMenuItem.Name = "saveSessionToolStripMenuItem";
            this.saveSessionToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.saveSessionToolStripMenuItem.Text = "&Save";
            this.saveSessionToolStripMenuItem.Click += new System.EventHandler(this.saveSessionToolStripMenuItem_Click);
            // 
            // saveSessionAsToolStripMenuItem
            // 
            this.saveSessionAsToolStripMenuItem.Name = "saveSessionAsToolStripMenuItem";
            this.saveSessionAsToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.saveSessionAsToolStripMenuItem.Text = "Save &as...";
            this.saveSessionAsToolStripMenuItem.Click += new System.EventHandler(this.saveSessionAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(133, 6);
            // 
            // sessionPropertiesToolStripMenuItem
            // 
            this.sessionPropertiesToolStripMenuItem.Name = "sessionPropertiesToolStripMenuItem";
            this.sessionPropertiesToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.sessionPropertiesToolStripMenuItem.Text = "&Properties...";
            this.sessionPropertiesToolStripMenuItem.Click += new System.EventHandler(this.sessionPropertiesToolStripMenuItem_Click);
            // 
            // logsToolStripMenuItem
            // 
            this.logsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLocalLogsToolStripMenuItem,
            this.addLogsFromSFTPToolStripMenuItem,
            this.toolStripMenuItem3,
            this.removeSelectedLogsToolStripMenuItem,
            this.clearLogsToolStripMenuItem,
            this.toolStripMenuItem4,
            this.logPropertiesToolStripMenuItem});
            this.logsToolStripMenuItem.Name = "logsToolStripMenuItem";
            this.logsToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.logsToolStripMenuItem.Text = "Logs";
            // 
            // addLocalLogsToolStripMenuItem
            // 
            this.addLocalLogsToolStripMenuItem.Name = "addLocalLogsToolStripMenuItem";
            this.addLocalLogsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.addLocalLogsToolStripMenuItem.Text = "Add &local...";
            this.addLocalLogsToolStripMenuItem.Click += new System.EventHandler(this.addLocalLogsToolStripMenuItem_Click);
            // 
            // addLogsFromSFTPToolStripMenuItem
            // 
            this.addLogsFromSFTPToolStripMenuItem.Name = "addLogsFromSFTPToolStripMenuItem";
            this.addLogsFromSFTPToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.addLogsFromSFTPToolStripMenuItem.Text = "Add from &sFTP...";
            this.addLogsFromSFTPToolStripMenuItem.Click += new System.EventHandler(this.addLogsFromSFTPToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(160, 6);
            // 
            // removeSelectedLogsToolStripMenuItem
            // 
            this.removeSelectedLogsToolStripMenuItem.Name = "removeSelectedLogsToolStripMenuItem";
            this.removeSelectedLogsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.removeSelectedLogsToolStripMenuItem.Text = "&Remove selected";
            this.removeSelectedLogsToolStripMenuItem.Click += new System.EventHandler(this.removeSelectedLogsToolStripMenuItem_Click);
            // 
            // clearLogsToolStripMenuItem
            // 
            this.clearLogsToolStripMenuItem.Name = "clearLogsToolStripMenuItem";
            this.clearLogsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.clearLogsToolStripMenuItem.Text = "&Clear";
            this.clearLogsToolStripMenuItem.Click += new System.EventHandler(this.clearLogsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(160, 6);
            // 
            // logPropertiesToolStripMenuItem
            // 
            this.logPropertiesToolStripMenuItem.Name = "logPropertiesToolStripMenuItem";
            this.logPropertiesToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.logPropertiesToolStripMenuItem.Text = "&Properties...";
            this.logPropertiesToolStripMenuItem.Click += new System.EventHandler(this.logPropertiesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(122, 6);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.optionsToolStripMenuItem.Text = "&Options...";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(122, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logFilesToolStripMenuItem,
            this.logEntriesToolStripMenuItem,
            this.messagesToolStripMenuItem,
            this.toolStripMenuItem9,
            this.filtersToolStripMenuItem,
            this.downloadQueueToolStripMenuItem,
            this.logCacheToolStripMenuItem,
            this.toolStripMenuItem8,
            this.fieldEditorToolStripMenuItem,
            this.formatEditorToolStripMenuItem,
            this.toolStripMenuItem10,
            this.selectColumnsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // logFilesToolStripMenuItem
            // 
            this.logFilesToolStripMenuItem.Checked = true;
            this.logFilesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.logFilesToolStripMenuItem.Name = "logFilesToolStripMenuItem";
            this.logFilesToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.logFilesToolStripMenuItem.Text = "&Log files";
            this.logFilesToolStripMenuItem.Click += new System.EventHandler(this.logFilesToolStripMenuItem_Click);
            // 
            // logEntriesToolStripMenuItem
            // 
            this.logEntriesToolStripMenuItem.Checked = true;
            this.logEntriesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.logEntriesToolStripMenuItem.Name = "logEntriesToolStripMenuItem";
            this.logEntriesToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.logEntriesToolStripMenuItem.Text = "Log &entries";
            this.logEntriesToolStripMenuItem.Click += new System.EventHandler(this.logEntriesToolStripMenuItem_Click);
            // 
            // messagesToolStripMenuItem
            // 
            this.messagesToolStripMenuItem.Checked = true;
            this.messagesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.messagesToolStripMenuItem.Name = "messagesToolStripMenuItem";
            this.messagesToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.messagesToolStripMenuItem.Text = "&Messages";
            this.messagesToolStripMenuItem.Click += new System.EventHandler(this.messagesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(170, 6);
            // 
            // filtersToolStripMenuItem
            // 
            this.filtersToolStripMenuItem.Name = "filtersToolStripMenuItem";
            this.filtersToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.filtersToolStripMenuItem.Text = "&Filters...";
            this.filtersToolStripMenuItem.Click += new System.EventHandler(this.filtersToolStripMenuItem_Click);
            // 
            // downloadQueueToolStripMenuItem
            // 
            this.downloadQueueToolStripMenuItem.Name = "downloadQueueToolStripMenuItem";
            this.downloadQueueToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.downloadQueueToolStripMenuItem.Text = "Download &queue...";
            this.downloadQueueToolStripMenuItem.Click += new System.EventHandler(this.downloadQueueToolStripMenuItem_Click);
            // 
            // logCacheToolStripMenuItem
            // 
            this.logCacheToolStripMenuItem.Name = "logCacheToolStripMenuItem";
            this.logCacheToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.logCacheToolStripMenuItem.Text = "Log &cache...";
            this.logCacheToolStripMenuItem.Click += new System.EventHandler(this.logCacheToolStripMenuItem_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(170, 6);
            // 
            // fieldEditorToolStripMenuItem
            // 
            this.fieldEditorToolStripMenuItem.Name = "fieldEditorToolStripMenuItem";
            this.fieldEditorToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.fieldEditorToolStripMenuItem.Text = "Field editor";
            this.fieldEditorToolStripMenuItem.Click += new System.EventHandler(this.fieldEditorToolStripMenuItem_Click);
            // 
            // formatEditorToolStripMenuItem
            // 
            this.formatEditorToolStripMenuItem.Name = "formatEditorToolStripMenuItem";
            this.formatEditorToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.formatEditorToolStripMenuItem.Text = "Format editor";
            this.formatEditorToolStripMenuItem.Click += new System.EventHandler(this.formatEditorToolStripMenuItem_Click);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(170, 6);
            // 
            // selectColumnsToolStripMenuItem
            // 
            this.selectColumnsToolStripMenuItem.Name = "selectColumnsToolStripMenuItem";
            this.selectColumnsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.selectColumnsToolStripMenuItem.Text = "Select columns...";
            this.selectColumnsToolStripMenuItem.Click += new System.EventHandler(this.selectColumnsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // mainStatusStrip
            // 
            this.mainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainStripStatusLabel});
            this.mainStatusStrip.Location = new System.Drawing.Point(0, 449);
            this.mainStatusStrip.Name = "mainStatusStrip";
            this.mainStatusStrip.Size = new System.Drawing.Size(771, 22);
            this.mainStatusStrip.TabIndex = 1;
            this.mainStatusStrip.Text = "statusStrip1";
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainSplitContainer.Location = new System.Drawing.Point(0, 27);
            this.mainSplitContainer.Name = "mainSplitContainer";
            this.mainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.logsListView);
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.entriesListView);
            this.mainSplitContainer.Size = new System.Drawing.Size(771, 419);
            this.mainSplitContainer.SplitterDistance = 298;
            this.mainSplitContainer.TabIndex = 2;
            // 
            // logsListView
            // 
            this.logsListView.AllowColumnReorder = true;
            this.logsListView.AllowDrop = true;
            this.logsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logsListView.FullRowSelect = true;
            this.logsListView.GridLines = true;
            this.logsListView.Location = new System.Drawing.Point(0, 0);
            this.logsListView.Name = "logsListView";
            this.logsListView.ShowGroups = false;
            this.logsListView.Size = new System.Drawing.Size(771, 298);
            this.logsListView.TabIndex = 0;
            this.logsListView.UseCompatibleStateImageBehavior = false;
            this.logsListView.View = System.Windows.Forms.View.Details;
            this.logsListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.logsListView_ColumnClick);
            this.logsListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.logsListView_DragDrop);
            this.logsListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.logsListView_DragEnter);
            this.logsListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.logsListView_KeyDown);
            this.logsListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.logsListView_MouseClick);
            // 
            // entriesListView
            // 
            this.entriesListView.AllowColumnReorder = true;
            this.entriesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entriesListView.FullRowSelect = true;
            this.entriesListView.GridLines = true;
            this.entriesListView.Location = new System.Drawing.Point(0, 0);
            this.entriesListView.Name = "entriesListView";
            this.entriesListView.ShowGroups = false;
            this.entriesListView.Size = new System.Drawing.Size(771, 117);
            this.entriesListView.TabIndex = 0;
            this.entriesListView.UseCompatibleStateImageBehavior = false;
            this.entriesListView.View = System.Windows.Forms.View.Details;
            // 
            // messageTimer
            // 
            this.messageTimer.Interval = 900;
            this.messageTimer.Tick += new System.EventHandler(this.messageTimer_Tick);
            // 
            // messagesPanel
            // 
            this.messagesPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messagesPanel.Controls.Add(this.messagesTextBox);
            this.messagesPanel.Location = new System.Drawing.Point(0, 269);
            this.messagesPanel.Name = "messagesPanel";
            this.messagesPanel.Size = new System.Drawing.Size(771, 177);
            this.messagesPanel.TabIndex = 3;
            // 
            // messagesTextBox
            // 
            this.messagesTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messagesTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messagesTextBox.Location = new System.Drawing.Point(0, 0);
            this.messagesTextBox.MaxLength = 65535;
            this.messagesTextBox.Multiline = true;
            this.messagesTextBox.Name = "messagesTextBox";
            this.messagesTextBox.ReadOnly = true;
            this.messagesTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.messagesTextBox.Size = new System.Drawing.Size(771, 177);
            this.messagesTextBox.TabIndex = 0;
            this.messagesTextBox.WordWrap = false;
            // 
            // mainStripStatusLabel
            // 
            this.mainStripStatusLabel.Name = "mainStripStatusLabel";
            this.mainStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 471);
            this.Controls.Add(this.messagesPanel);
            this.Controls.Add(this.mainSplitContainer);
            this.Controls.Add(this.mainStatusStrip);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "MainForm";
            this.Text = "Lumberjack";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_OnLoad);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.mainStatusStrip.ResumeLayout(false);
            this.mainStatusStrip.PerformLayout();
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            this.messagesPanel.ResumeLayout(false);
            this.messagesPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem openSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem saveSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSessionAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem sessionPropertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addLocalLogsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addLogsFromSFTPToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem removeSelectedLogsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearLogsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem logPropertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadQueueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logCacheToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem fieldEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logEntriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem messagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem filtersToolStripMenuItem;
        private System.Windows.Forms.StatusStrip mainStatusStrip;
        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.ListView logsListView;
        private System.Windows.Forms.ListView entriesListView;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem10;
        private System.Windows.Forms.ToolStripMenuItem selectColumnsToolStripMenuItem;
        private System.Windows.Forms.Timer messageTimer;
        private System.Windows.Forms.Panel messagesPanel;
        private System.Windows.Forms.TextBox messagesTextBox;
        private System.Windows.Forms.ToolStripStatusLabel mainStripStatusLabel;
    }
}

