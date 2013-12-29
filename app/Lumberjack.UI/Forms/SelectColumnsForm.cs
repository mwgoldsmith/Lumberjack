using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Medidata.Lumberjack.Core;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.UI.Properties;

namespace Medidata.Lumberjack.UI
{
    public partial class SelectColumnsForm : Form
    {
        #region Private fields

        private static readonly string[] InternalLogFileColumns = new[]
            {
                "Filename",
                "Size",
                "Format",
                "MD5 Hash",
                "Total Entries",
                "First Entry", 
                "Last Entry",
                "DEBUG", 
                "INFO",
                "WARN",
                "ERROR",
                "TRACE",
                "FATAL"
            };

        private readonly UserSession _session;

        private FieldContextFlags _curListCols;

        private List<string> _logsChecked = new List<string>();
        private List<string> _entriesChecked = new List<string>();

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        public SelectColumnsForm() {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="logKey"></param>
        /// <param name="entryKey"></param>
        public SelectColumnsForm(UserSession session, string logKey, string entryKey)
            : this() {// IEnumerable<string> logColumns, IEnumerable<string> entryColumns)
            
            _session = session;
            var x = Settings.Default[logKey].ToString();
            // Create each column of the specified width and set the Tag value
            var cols = x.Split(',');
            foreach (var col in cols) {
                _logsChecked.Add(col.Split(':')[0]);
            }

            cols = Settings.Default[entryKey].ToString().Split(',');
            foreach (var col in cols) {
                _entriesChecked.Add(col.Split(':')[0]);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public List<string> LogFileColumns {
            get { return _logsChecked; }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> LogEntryColumns {
            get { return _entriesChecked; }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectColumnsForm_Load(object sender, EventArgs e) {
            listViewComboBox.SelectedItem = "Log files";
            _curListCols = FieldContextFlags.Filename;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e) {
            if (_curListCols == FieldContextFlags.Entry) {
                _entriesChecked = GetCheckedItems();
            } else if (_curListCols == FieldContextFlags.Filename) {
                _logsChecked = GetCheckedItems();
            }

            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e) {
            _logsChecked = null;
            _entriesChecked = null;

            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            var text = listViewComboBox.SelectedItem.ToString();
            var checks = new List<string>();
            var fields = new List<string>();

            // Save the previous selection's checkboxes
            if (_curListCols == FieldContextFlags.Entry) {
                _entriesChecked = GetCheckedItems();
            } else if (_curListCols == FieldContextFlags.Filename) {
                _logsChecked = GetCheckedItems();
            }

            if (text.Equals("Log files")) {
                _curListCols = FieldContextFlags.Filename;

                checks.AddRange(_logsChecked);
                fields.AddRange(InternalLogFileColumns);
                fields.AddRange(GetColumnDisplays(_curListCols));
            } else if (text.Equals("Log entries")) {
                _curListCols = FieldContextFlags.Entry;

                checks.AddRange(_entriesChecked);
                fields.AddRange(GetColumnDisplays(_curListCols | FieldContextFlags.Content));
            }


            columnsCheckedListBox.Items.Clear();
            foreach (var d in fields) {
                var isChecked = checks.Exists(x => x.Equals(d));

                columnsCheckedListBox.Items.Add(d, isChecked);
            }
        }

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<string> GetCheckedItems() {
            var checkedItems = columnsCheckedListBox.CheckedItems;

            return (from object c in checkedItems select c.ToString()).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerType"></param>
        /// <returns></returns>
        private IEnumerable<string> GetColumnDisplays(FieldContextFlags containerType) {
            var fields = _session.SessionFields.FindAll(containerType);

            return (from f in fields where f.Display != null select f.Display).ToList();
        }
        
        #endregion

    }
}
