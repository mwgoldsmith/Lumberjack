using System.Collections;
using System.Windows.Forms;

namespace Medidata.Lumberjack.UI
{
    #region Delegates

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="colToSort"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public delegate int CompareHandler(ListViewColumnSorter sender, int colToSort, ListViewItem x, ListViewItem y);

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public class ListViewColumnSorter : IComparer
    {
        #region Initializers

        /// <summary>
        /// Class constructor.  Initializes various elements
        /// </summary>
        public ListViewColumnSorter() {
            SortColumn = 0;
            Order = SortOrder.None;
            ObjectCompare = new CaseInsensitiveComparer();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn { get; set; }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CaseInsensitiveComparer ObjectCompare { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CompareHandler OnCompare { get; set; }

        #endregion

        #region IComparer implementation

        /// <summary>
        /// This method is inherited from the IComparer interface. It compares the
        /// two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>
        /// The result of the comparison. "0" if equal, negative if 'x' is less than
        /// 'y' and positive if 'x' is greater than 'y'
        /// </returns>
        public int Compare(object x, object y) {
            var textX = ((ListViewItem)x).SubItems[SortColumn].Text;
            var textY = ((ListViewItem)y).SubItems[SortColumn].Text;

            var compareResult = OnCompare != null
                ? OnCompare(this, SortColumn, (ListViewItem)x, (ListViewItem)y)
                : ObjectCompare.Compare(textX, textY);

            if (Order == SortOrder.Ascending) {
                return compareResult;
            }

            if (Order == SortOrder.Descending) {
                return (-compareResult);
            }

            return 0;
        }

        #endregion
    }
}
