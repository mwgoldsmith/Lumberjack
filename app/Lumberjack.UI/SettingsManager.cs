using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Medidata.Lumberjack.Core.Logging;
using Medidata.Lumberjack.UI.Properties;

namespace Medidata.Lumberjack.UI
{
    #region

    /// <summary>
    /// 
    /// </summary>
    internal struct ListViewColumn
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="width"></param>
        public ListViewColumn(string text, int width)
            : this() {
            Text = text;
            Width = width;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Width { get; private set; }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class LoggingLevels
    {
        public bool TraceEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool DebugEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool InfoEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool WarnEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ErrorEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool FineEnabled { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class ConfigFilenames
    {
        /// <summary>
        /// 
        /// </summary>
        public string Fields { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Formats { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Nodes { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class DisplayState
    {
        /// <summary>
        /// 
        /// </summary>
        public bool ShowMessages { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowLogFiles { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowEntries { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SplitterDistance { get; set; }
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    internal static class SettingsManager
    {
        #region Private fields

        private static readonly Regex _regexColumns = new Regex(@"([^:]+):(\d+),?", RegexOptions.IgnoreCase);

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="getDefault"></param>
        /// <returns></returns>
        public static T GetSetting<T>(string key, bool getDefault = false) {
            T result;

            try {
                if (!getDefault) {
                    result = (T) Settings.Default[key];
                } else {
                    var prop = Settings.Default.Properties[key];
                    result = prop != null ? (T) prop.DefaultValue : default(T);
                }

                Logger.DefaultLogger.Trace("Loaded setting : " + result + (getDefault ? " [DEFAULT]" : ""));
            } catch (Exception ex) {
                Logger.DefaultLogger.Error("Failed to retrieve settings value of type " + typeof (T), ex);

                return default(T);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ConfigFilenames LoadConfigFilenames() {
            return new ConfigFilenames
                {
                    Fields = GetSetting<string>("FieldsFilename"),
                    Formats = GetSetting<string>("FormatsFilename"),
                    Nodes = GetSetting<string>("NodesFilename"),
                };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DisplayState LoadDisplayState() {
            return new DisplayState
                {
                    ShowMessages = GetSetting<bool>("ShowMessages"),
                    ShowLogFiles = GetSetting<bool>("ShowLogFiles"),
                    ShowEntries = GetSetting<bool>("ShowEntries"),
                    SplitterDistance = GetSetting<int>("SplitterDistance")
                };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<ListViewColumn> LoadEntriesColumns() {
            var result = ParseColumns(GetSetting<string>("EntriesListViewColumns"));
            if (result != null && result.Count > 0)
                return result;

            return ParseColumns(GetSetting<string>("EntriesListViewColumns", true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<ListViewColumn> LoadLogFilesColumns() {
            var result = ParseColumns(GetSetting<string>("LogFilesListViewColumns"));
            if (result != null && result.Count > 0)
                return result;

            return ParseColumns(GetSetting<string>("LogFilesListViewColumns", true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static LoggingLevels LoadLoggingLevels() {
            return new LoggingLevels
                {
                    DebugEnabled = GetSetting<bool>("DebugLoggingEnabled"),
                    ErrorEnabled = GetSetting<bool>("ErrorLoggingEnabled"),
                    FineEnabled = GetSetting<bool>("FineLoggingEnabled"),
                    InfoEnabled = GetSetting<bool>("InfoLoggingEnabled"),
                    WarnEnabled = GetSetting<bool>("WarnLoggingEnabled"),
                    TraceEnabled = GetSetting<bool>("TraceLoggingEnabled")
                };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public static void SaveConfigFilenames(ConfigFilenames values) {
            SetSetting("FieldsFilename", values.Fields);
            SetSetting("FormatsFilename", values.Formats);
            SetSetting("NodesFilename", values.Nodes, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static void SaveDisplayState(DisplayState values) {
            SetSetting("ShowMessages", values.ShowMessages);
            SetSetting("ShowLogFiles", values.ShowLogFiles);
            SetSetting("ShowEntries", values.ShowEntries);
            SetSetting("SplitterDistance", values.SplitterDistance, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static void SaveLoggingLevels(LoggingLevels values) {
            SetSetting("DebugLoggingEnabled", values.DebugEnabled);
            SetSetting("ErrorLoggingEnabled", values.ErrorEnabled);
            SetSetting("FineLoggingEnabled", values.FineEnabled);
            SetSetting("InfoLoggingEnabled", values.InfoEnabled);
            SetSetting("WarnLoggingEnabled", values.WarnEnabled);
            SetSetting("TraceLoggingEnabled", values.TraceEnabled, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="save"></param>
        public static void SetSetting<T>(string key, T value, bool save = false) {
            if (!IsSettingKey(key))
                return;

            try {
                Settings.Default[key] = value;

                if (save)
                    Settings.Default.Save();
            } catch (Exception ex) {
                Logger.DefaultLogger.Error("Failed to save settings", ex);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">The name of the key to test.</param>
        /// <returns>True if the given key is a valid settings key; otherwise, false.</returns>
        private static bool IsSettingKey(string key) {
            var props = Settings.Default.Properties;
            var array = new SettingsProperty[props.Count];

            props.CopyTo(array, 0);

            return array.Any(t => t.Name.Equals(key));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text">Text to parse. Format is: name:width[,...]</param>
        /// <returns>
        /// List of columns (with name and width values) if successful;
        /// otherwise, null.
        /// </returns>
        private static List<ListViewColumn> ParseColumns(string text) {
            var result = new List<ListViewColumn>();
            var matches = _regexColumns.Matches(text ?? "");

            try {
                foreach (Match match in matches) {
                    if (!match.Success)
                        continue;

                    var name = match.Groups[1].ToString();
                    var width = Int32.Parse(match.Groups[2].ToString());

                    result.Add(new ListViewColumn(name, width));
                }
            } catch (Exception ex) {
                // This should really never occur since the Regex will only match
                // valid values. However, if something magically goes wrong lets
                // do it gracefully

                Logger.DefaultLogger.Error("Failed to parse settings value", ex);
                return null;
            }

            return result;
        }

        #endregion
    }
}