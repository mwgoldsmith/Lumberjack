using System;
using System.Windows.Forms;

namespace Medidata.Lumberjack
{
    public partial class AboutForm : Form
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        public AboutForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Control event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion
    }
}
