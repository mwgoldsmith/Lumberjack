using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medidata.Lumberjack.Logging;

namespace Medidata.Lumberjack
{
    public partial class PropertiesForm : Form
    {
        #region Private Fields

        private List<Log> _logs = null; 
        
        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        public PropertiesForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logs"></param>
        public PropertiesForm(List<Log> logs)
            : this()
        {
            _logs = logs;
        }

        #endregion

    }
}
