using System;
using System.IO;
using System.Text.RegularExpressions;
using Medidata.Lumberjack.Core.Logging;

namespace Medidata.Lumberjack.Core
{
    /// <summary>
    /// 
    /// </summary>
    internal static class FileUtil
    {
        #region Private fields

        private static readonly Regex _regexValidatePath = new Regex(@"(\b[a-z]:|\\\\[a-z0-9 %._-]+\\[a-z0-9 $%._-]+)\\((?:[^\\/:*?""<>|\r\n]+\\)*)(?:[^\\/:*?""<>|\r\n]*)", RegexOptions.IgnoreCase);
        private static readonly string[] _drives;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        static FileUtil() {
            _drives = Directory.GetLogicalDrives();
            for (var i = 0; i < _drives.Length; i++)
                _drives[i] = _drives[i].Substring(0, 1).ToUpper();

            ApplicationPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        #endregion

        #region Private fields

        /// <summary>
        /// 
        /// </summary>
        public static string ApplicationPath { get; set; }

        #endregion

        #region Private fields

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetAbsoluteFilename(string filename) {
            var path = Path.GetDirectoryName(Path.GetFullPath(filename));
            var file = Path.GetFileName(filename);

            return GetAbsoluteFilename(path, file);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetAbsoluteFilename(string path, string filename) {
            var dir = String.IsNullOrEmpty(path) ? ApplicationPath : GetEnsuredPath(path);

            // Condition should never occur where path is null
            return dir != null ? Path.Combine(dir, filename) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetEnsuredPath(string directory) {
            string path = null;

            var match = _regexValidatePath.Match(directory);
            if (!match.Success)
                return null;

            var drive = match.Groups[1].Value;
            if (drive.Length < 1)
                return null;

            if (drive.Length == 2) {
                if (drive.Substring(1, 1).Equals(":")) {
                    drive = drive.Substring(0, 1);
                    if (Array.IndexOf(_drives, drive) == -1)
                        return null;
                }
            }

            try {
                if (!Directory.Exists(directory))
                    path = Path.GetDirectoryName(directory);
                
                if (path != null && !Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }

                path = directory;
            } catch (UnauthorizedAccessException) {
                Logger.GetInstance().Error("Unauthorized access to path: \"" + path + "\".");
                path = null;
            } catch (IOException ex) {
                Logger.GetInstance().Error("An IO exception occured while attempting create the path for: \"" + directory + "\"", ex);
                path = null;
            }

            return path;
        }

        #endregion
    }
}
