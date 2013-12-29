namespace Medidata.Lumberjack.UI
{
    partial class SelectColumnsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new System.Windows.Forms.Label();
            this.listViewComboBox = new System.Windows.Forms.ComboBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.columnsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Columns for List View:";
            // 
            // listViewComboBox
            // 
            this.listViewComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listViewComboBox.FormattingEnabled = true;
            this.listViewComboBox.Items.AddRange(new object[] {
            "Log files",
            "Log entries"});
            this.listViewComboBox.Location = new System.Drawing.Point(133, 6);
            this.listViewComboBox.Name = "listViewComboBox";
            this.listViewComboBox.Size = new System.Drawing.Size(136, 21);
            this.listViewComboBox.TabIndex = 1;
            this.listViewComboBox.SelectedIndexChanged += new System.EventHandler(this.listViewComboBox_SelectedIndexChanged);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(113, 227);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(194, 227);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // columnsCheckedListBox
            // 
            this.columnsCheckedListBox.CheckOnClick = true;
            this.columnsCheckedListBox.FormattingEnabled = true;
            this.columnsCheckedListBox.Items.AddRange(new object[] {
            "Filename",
            "Size"});
            this.columnsCheckedListBox.Location = new System.Drawing.Point(12, 33);
            this.columnsCheckedListBox.Name = "columnsCheckedListBox";
            this.columnsCheckedListBox.Size = new System.Drawing.Size(257, 184);
            this.columnsCheckedListBox.TabIndex = 4;
            // 
            // SelectColumnsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.columnsCheckedListBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.listViewComboBox);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectColumnsForm";
            this.Text = "SelectColumnsForm";
            this.Load += new System.EventHandler(this.SelectColumnsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox listViewComboBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckedListBox columnsCheckedListBox;
    }
}