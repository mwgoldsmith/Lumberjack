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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(445, 323);
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
            this.logPropertyListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
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
            this.logFileTextBox.Size = new System.Drawing.Size(458, 20);
            this.logFileTextBox.TabIndex = 3;
            // 
            // logValueTextBox
            // 
            this.logValueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logValueTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logValueTextBox.Location = new System.Drawing.Point(148, 32);
            this.logValueTextBox.Multiline = true;
            this.logValueTextBox.Name = "logValueTextBox";
            this.logValueTextBox.ReadOnly = true;
            this.logValueTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logValueTextBox.Size = new System.Drawing.Size(372, 285);
            this.logValueTextBox.TabIndex = 4;
            this.logValueTextBox.WordWrap = false;
            // 
            // editButton
            // 
            this.editButton.Enabled = false;
            this.editButton.Location = new System.Drawing.Point(12, 322);
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
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Session Level",
            "Format Level"});
            this.comboBox1.Location = new System.Drawing.Point(53, 156);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(89, 21);
            this.comboBox1.TabIndex = 8;
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(12, 183);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(130, 133);
            this.listView1.TabIndex = 9;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // LogPropertiesForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(532, 358);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.logValueTextBox);
            this.Controls.Add(this.logFileTextBox);
            this.Controls.Add(this.logPropertyListBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okButton);
            this.MaximizeBox = false;
            this.Name = "LogPropertiesForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Log Properties";
            this.Load += new System.EventHandler(this.LogPropertiesForm_Load);
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
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ListView listView1;
    }
}