using System.Diagnostics;
using System.Threading;

namespace Medidata.Lumberjack.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    internal static class KeyGenerator
    {
        #region Private fields

        private static long _id;

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        internal static int GetNextId() {
            return (int)Interlocked.Increment(ref _id);
        }

        #endregion
    }
}
