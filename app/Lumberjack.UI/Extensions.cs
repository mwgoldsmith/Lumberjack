using System;
using System.Windows.Forms;

namespace Medidata.Lumberjack.UI
{
    /// <summary>
    /// 
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        internal static int IndexOfBreak(this string str, out int length) {
            return IndexOfBreak(str, 0, out length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        internal static int IndexOfBreak(this string str, int startIndex, out int length) {
            if (String.IsNullOrEmpty(str)) {
                length = 0;
                return -1;
            }

            var ub = str.Length - 1;
            if (startIndex > ub) {
                throw new ArgumentOutOfRangeException();
            }

            for (var i = startIndex; i <= ub; i++) {
                int intchr = str[i];
                if (intchr == 0x0D) {
                    if (i < ub && str[i + 1] == 0x0A) {
                        length = 2;
                    } else {
                        length = 1;
                    }
                    return i;
                }

                if (intchr != 0x0A)
                    continue;

                length = 1;
                return i;
            }

            length = 0;
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="count"></param>
        internal static void ClearTopLines(this TextBox control, int count) {
            if (count <= 0) {
                return;
            } 
            
            if (!control.Multiline) {
                control.Clear();
                return;
            }

            var txt = control.Text;
            var cursor = 0;
            var brkCount = 0;

            while (brkCount < count) {
                int brkLength;
                var ixOf = txt.IndexOfBreak(cursor, out brkLength);

                if (ixOf < 0) {
                    control.Clear();
                    return;
                }
                cursor = ixOf + brkLength;
                brkCount++;
            }

            control.Text = txt.Substring(cursor);
        }
        
        /// <summary>
        /// Execute a method on the control's owning thread.
        /// </summary>
        /// <param name="source">The control that is being updated.</param>
        /// <param name="updater">The method that updates uiElement.</param>
        /// <param name="forceSynchronous">True to force synchronous execution of 
        /// updater.  False to allow asynchronous execution if the call is marshalled
        /// from a non-GUI thread.  If the method is called on the GUI thread,
        /// execution is always synchronous.</param>
        internal static void Invoke(this Control source, Action updater, bool forceSynchronous) {
            if (source == null) {
                throw new ArgumentNullException();
            }

            if (source.InvokeRequired) {
                if (forceSynchronous) {
                    source.Invoke((Action) (() => Invoke(source, updater, true)));
                } else {
                    source.BeginInvoke((Action)(() => Invoke(source, updater, false)));
                }
            } else {
                if (!source.IsHandleCreated) {
                    // Do nothing if the handle isn't created already.  The user's responsible
                    // for ensuring that the handle they give us exists.
                    return;
                }

                if (source.IsDisposed) {
                    throw new ObjectDisposedException("Control is already disposed.");
                }

                updater();
            }
        }
    }
}
