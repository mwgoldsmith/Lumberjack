using System;
using System.Configuration;
using Medidata.Lumberjack.UI.Properties;

namespace Medidata.Lumberjack.UI
{
    internal static class SettingsUtil
    {
        public static bool IsSettingKey(string key) {
            var props = Settings.Default.Properties;
            var array = new SettingsProperty[props.Count];

            props.CopyTo(array, 0);

            for (var i = 0; i < array.Length; i++)
                if (array[i].Name.Equals(key)) {
                    return true;
                }

            return false;
        }

        public static void SetSettingValue(string key, string value) {
            SetSettingValue(key, value, false);

        }

        public static void SetSettingValue(string key, string value, bool autoSave) {
            if (!IsSettingKey(key))
                return;

            Settings.Default[key] = value;

            if (autoSave)
                Settings.Default.Save();
        }

        public static string GetSettingOrDefault(string key) {
            return GetSettingOrDefault(key, false);
        }

        public static string GetSettingOrDefault(string key, bool autoSave) {
            if (!IsSettingKey(key))
                return null;

            var value = Settings.Default[key].ToString();

            if (String.IsNullOrWhiteSpace(value)) {
                var prop = Settings.Default.Properties[key];

                if (prop != null) {
                    value = prop.DefaultValue.ToString();
                    Settings.Default[key] = value;
                    Settings.Default.Save();
                }
            }

            return value;
        }

        public static string RestoreDefaultSetting(string key) {
            if (!IsSettingKey(key))
                return null;

            var prop = Settings.Default.Properties[key];

            if (prop != null) {
                var value = prop.DefaultValue.ToString();
                Settings.Default[key] = value;
                Settings.Default.Save();

                return value;
            }

            return null;
        }
    }
}

