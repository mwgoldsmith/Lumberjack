using System;
using System.IO;
using System.Xml.Serialization;
using Medidata.Lumberjack.Properties;

namespace Medidata.Lumberjack.Logging.Config
{
    // TODO: Clean up the design for configurables in the future (possibly a custom IXmlSerializable implementation?)

    /// <summary>
    /// 
    /// </summary>
    public abstract class ConfigurableBase : MessageEmitterBase
    {
        #region Private fields

        protected static readonly object _locker = new object();

        private readonly string _settingsKey;

        private static string _configPath;

        private string _filename;
        private volatile bool _isConfigured;
        private volatile bool _isDirty;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settingsKey"></param>
        protected ConfigurableBase(string settingsKey)
        {
            _settingsKey = settingsKey;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public string ConfigFilename
        {
            get { lock (_locker) return _filename; }
            set { lock (_locker) _filename = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public bool IsConfigured
        {
            get { lock (_locker) return _isConfigured; }
            set { lock (_locker) _isConfigured = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        protected bool IsDirty
        {
            get { lock (_locker) return _isDirty; }
            set { lock (_locker) _isDirty = value; }
        }


        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ConfigurableBase Load()
        {
            object config;
            var type = GetType();

            try
            {
                var filename = GetConfigFilename(_settingsKey);

                lock (_locker)
                {
                    _isConfigured = false;

                    // Read the xml config file and deserialize it
                    using (var stream = new StreamReader(filename))
                    {
                        var serializer = new XmlSerializer(type);
                        config = serializer.Deserialize(stream);
                    }

                    Initialize((ConfigurableBase)config);

                    _isDirty = false;
                    _isConfigured = true;
                }
            }
            catch (IOException ex)
            {
                config = null;
                ErrorMessage(ex);
            }
            catch (SystemException ex)
            {
                config = null;
                ErrorMessage(ex);
            }
            catch (Exception ex)
            {
                config = null;
                ErrorMessage(ex);
            }

            return (ConfigurableBase)config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>If successful, true; otherwise, false.</returns>
        public virtual bool Save()
        {
            var success = false;
            var type = GetType();

            try
            {
                var filename = GetConfigFilename(_settingsKey);

                lock (_locker)
                {
                    using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        var serializer = new XmlSerializer(type);
                        serializer.Serialize(fs, this);
                        fs.Close();
                    }
                }

                _isDirty = false;
                success = true;
            }
            catch (IOException ex)
            {
                ErrorMessage(ex);
                //, "An IO exception occured while saving configuration for " + type.FullName + ".");
            }
            catch (SystemException ex)
            {
                ErrorMessage(ex);//, "A system exception occurred while saving configuration for " + type.FullName + ".");
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);//, "Failed to save configuration for " + type.FullName + ".");
            }

            return success;
        }

        #endregion

        #region Protected abstract methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        protected abstract void Initialize(ConfigurableBase config);

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetConfigFilename(string key)
        {
            var value = Settings.Default[key];
            if (value == null) return null;

            // Does the filename from settings contain a path?
            var path = Path.GetDirectoryName(value.ToString());
            if (String.IsNullOrEmpty(path))
            {
                // Use application directory if no other directory is set
                if (String.IsNullOrEmpty(_configPath))
                {
                    _configPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                }
                
                path = _configPath;

                // Oh well... we tried
                if (path == null) return null;
            }

            // Create it if it doesnt exist
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return (path + "/" + value).Replace('/', Path.DirectorySeparatorChar);
        }

        #endregion
    }
}
