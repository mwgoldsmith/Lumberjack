namespace Medidata.Lumberjack.UI
{
    partial class LogPropertiesForm
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
            this.okButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.logPropertyListBox = new System.Windows.Forms.ListBox();
            this.logFileTextBox = new System.Windows.Forms.TextBox();
            this.logValueTextBox = new System.Windows.Forms.TextBox();
            this.editButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.fieldLevelComboBox = new System.Windows.Forms.ComboBox();
            this.formatFieldTreeView = new System.Windows.Forms.TreeView();
            this.sessionFieldListBox = new System.Windows.Forms.ListBox();
            this.editPanel = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.valueComboBox = new System.Windows.Forms.ComboBox();
            this.valueTextBox = new System.Windows.Forms.TextBox();
            this.valueLabel = new System.Windows.Forms.Label();
            this.editPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(507, 349);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "Ok";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Log file:";
            // 
            // logPropertyListBox
            // 
            this.logPropertyListBox.FormattingEnabled = true;
            this.logPropertyListBox.Location = new System.Drawing.Point(12, 32);
            this.logPropertyListBox.Name = "logPropertyListBox";
            this.logPropertyListBox.Size = new System.Drawing.Size(130, 121);
            this.logPropertyListBox.TabIndex = 2;
            this.logPropertyListBox.SelectedIndexChanged += new System.EventHandler(this.logPropertyListBox_SelectedIndexChanged);
            // 
            // logFileTextBox
            // 
            this.logFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logFileTextBox.Location = new System.Drawing.Point(62, 6);
            this.logFileTextBox.Name = "logFileTextBox";
            this.logFileTextBox.ReadOnly = true;
            this.logFileTextBox.Size = new System.Drawing.Size(520, 20);
            this.logFileTextBox.TabIndex = 3;
            // 
            // logValueTextBox
            // 
            this.logValueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logValueTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logValueTextBox.Location = new System.Drawing.Point(148, 75);
            this.logValueTextBox.Multiline = true;
            this.logValueTextBox.Name = "logValueTextBox";
            this.logValueTextBox.ReadOnly = true;
            this.logValueTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logValueTextBox.Size = new System.Drawing.Size(434, 266);
            this.logValueTextBox.TabIndex = 4;
            this.logValueTextBox.WordWrap = false;
            // 
            // editButton
            // 
            this.editButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.editButton.Enabled = false;
            this.editButton.Location = new System.Drawing.Point(12, 349);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(75, 23);
            this.editButton.TabIndex = 5;
            this.editButton.Text = "Edit";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Fields:";
            // 
            // fieldLevelComboBox
            // 
            this.fieldLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fieldLevelComboBox.FormattingEnabled = true;
            this.fieldLevelComboBox.Items.AddRange(new object[] {
            "Session Level",
            "Format Level"});
            this.fieldLevelComboBox.Location = new System.Drawing.Point(53, 156);
            this.fieldLevelComboBox.Name = "fieldLevelComboBox";
            this.fieldLevelComboBox.Size = new System.Drawing.Size(89, 21);
            this.fieldLevelComboBox.TabIndex = 8;
            this.fieldLevelComboBox.SelectedIndexChanged += new System.EventHandler(this.fieldLevelComboBox_SelectedValueChanged);
            // 
            // formatFieldTreeView
            // 
            this.formatFieldTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.formatFieldTreeView.Location = new System.Drawing.Point(12, 183);
            this.formatFieldTreeView.Name = "formatFieldTreeView";
            this.formatFieldTreeView.Size = new System.Drawing.Size(130, 158);
            this.formatFieldTreeView.TabIndex = 9;
            this.formatFieldTreeView.LocationChanged += new System.EventHandler(this.formatFieldTreeView_LocationChanged);
            // 
            // sessionFieldListBox
            // 
            this.sessionFieldListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.sessionFieldListBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.sessionFieldListBox.FormattingEnabled = true;
            this.sessionFieldListBox.IntegralHeight = false;
            this.sessionFieldListBox.Location = new System.Drawing.Point(12, 183);
            this.sessionFieldListBox.Name = "sessionFieldListBox";
            this.sessionFieldListBox.Size = new System.Drawing.Size(130, 158);
            this.sessionFieldListBox.TabIndex = 10;
            this.sessionFieldListBox.SelectedIndexChanged += new System.EventHandler(this.sessionFieldListBox_SelectedIndexChanged);
            // 
            // editPanel
            // 
            this.editPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editPanel.Controls.Add(this.saveButton);
            this.editPanel.Controls.Add(this.cancelButton);
            this.editPanel.Controls.Add(this.valueComboBox);
            this.editPanel.Controls.Add(this.valueTextBox);
            this.editPanel.Controls.Add(this.valueLabel);
            this.editPanel.Location = new System.Drawing.Point(148, 32);
            this.editPanel.Name = "editPanel";
            this.editPanel.Size = new System.Drawing.Size(434, 37);
            this.editPanel.TabIndex = 11;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(275, 36);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 3;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(356, 36);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // valueComboBox
            // 
            this.valueComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valueComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.valueComboBox.Enabled = false;
            this.valueComboBox.FormattingEnabled = true;
            this.valueComboBox.Location = new System.Drawing.Point(50, 9);
            this.valueComboBox.Name = "valueComboBox";
            this.valueComboBox.Size = new System.Drawing.Size(381, 21);
            this.valueComboBox.TabIndex = 1;
            this.valueComboBox.Visible = false;
            // 
            // valueTextBox
            // 
            this.valueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valueTextBox.Location = new System.Drawing.Point(50, 9);
            this.valueTextBox.Name = "valueTextBox";
            this.valueTextBox.ReadOnly = true;
            this.valueTextBox.Size = new System.Drawing.Size(381, 20);
            this.valueTextBox.TabIndex = 1;
            this.valueTextBox.Visible = false;
            // 
            // valueLabel
            // 
            this.valueLabel.AutoSize = true;
            this.valueLabel.Location = new System.Drawing.Point(7, 12);
            this.valueLabel.Name = "valueLabel";
            this.valueLabel.Size = new System.Drawing.Size(37, 13);
            this.valueLabel.TabIndex = 0;
            this.valueLabel.Text = "Value:";
            // 
            // LogPropertiesForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(594, 384);
            this.Controls.Add(this.editPanel);
            this.Controls.Add(this.sessionFieldListBox);
            this.Controls.Add(this.formatFieldTreeView);
            this.Controls.Add(this.fieldLevelComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.logValueTextBox);
            this.Controls.Add(this.logFileTextBox);
            this.Controls.Add(this.logPropertyListBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okButton);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(275, 310);
            this.Name = "LogPropertiesForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Log Properties";
            this.Load += new System.EventHandler(this.LogPropertiesForm_Load);
            this.editPanel.ResumeLayout(false);
            this.editPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox logPropertyListBox;
        private System.Windows.Forms.TextBox logFileTextBox;
        private System.Windows.Forms.TextBox logValueTextBox;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox fieldLevelComboBox;
        private System.Windows.Forms.TreeView formatFieldTreeView;
        private System.Windows.Forms.ListBox sessionFieldListBox;
        private System.Windows.Forms.Panel editPanel;
        private System.Windows.Forms.ComboBox valueComboBox;
        private System.Windows.Forms.TextBox valueTextBox;
        private System.Windows.Forms.Label valueLabel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
    }
}