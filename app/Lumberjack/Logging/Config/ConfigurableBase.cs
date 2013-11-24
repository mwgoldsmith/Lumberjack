using System;
using System.IO;
using System.Xml.Serialization;

namespace Medidata.Lumberjack.Logging.Config
{
    public abstract class ConfigurableBase : MessageEmitterBase
    {
        #region Private fields

        protected static readonly object _locker = new object();

        private string _filename;
        private volatile bool _isConfigured;
        private volatile bool _isDirty;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        protected ConfigurableBase(string filename)
        {
            _filename = filename;
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
        protected bool IsConfigured
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
        public virtual object Load()
        {
            object config = null;
            var type = GetType();

            try
            {
                lock (_locker)
                {
                    _isConfigured = false;

                    // Read the xml config file and deserialize it
                    using (var stream = new StreamReader(_filename))
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
                ErrorMessage(ex);
            }
            catch (SystemException ex)
            {
                ErrorMessage(ex);
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }

            return config;
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
                lock (_locker)
                {
                    using (var fs = new FileStream(_filename, FileMode.Create, FileAccess.Write, FileShare.None))
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
    }
}
