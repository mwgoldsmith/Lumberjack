using System.IO;
using System.Xml.Serialization;

namespace Medidata.Lumberjack.Core.Config
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ConfigurableBase : SerializableBase
    {
        #region Private fields

        protected static readonly object _locker = new object();

        private volatile bool _isConfigured;
        private volatile bool _isDirty;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        protected ConfigurableBase() : this(null) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        protected ConfigurableBase(UserSession session) : base(session) { }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        protected virtual string Filename { get; set; }

        /// <summary>
        /// If the configurator has been loaded successfully, true;
        /// otherwise, false.
        /// </summary>
        [XmlIgnore]
        public bool IsConfigured {
            get { lock (_locker) return _isConfigured; }
            set { lock (_locker) _isConfigured = value; }
        }

        /// <summary>
        /// If the configurator has been modified without being save, true;
        /// otherwise, false.
        /// </summary>
        [XmlIgnore]
        protected bool IsDirty {
            get { lock (_locker) return _isDirty; }
            set { lock (_locker) _isDirty = value; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Loads the configuration data from XML and deserializes it. 
        /// </summary>
        /// <returns>If successful, true; otherwise, false.</returns>
        public bool Load() {
            var filename = FileUtil.GetAbsoluteFilename(Filename);
            var success = !string.IsNullOrEmpty(filename);

            if (!success)
                return false;
            
            // If the file does not exist, create it be saving the configurator
            if (!File.Exists(filename)) {
                Save(filename);
            }

            lock (_locker) {
                _isConfigured = false;

                var config = Deserialize(filename);
                success = config != null;

                // Allow the configurable to initialize even on failure
                Initialize((ConfigurableBase) config);

                // The configurable is no longer dirty if successful
                _isDirty = !success;
                _isConfigured = success;

                if (success) {
                    OnInfo("Configuration loaded for " + GetType().Name + " successfully.");
                } else {
                    OnInfo("Failed to load configurable for " + GetType().Name + ".");
                }
            }

            return success;
        }

        /// <summary>
        /// Saves the configuration data as serialized XML.
        /// </summary>
        /// <returns>If successful, true; otherwise, false.</returns>
        public bool Save() {
            var filename = FileUtil.GetAbsoluteFilename(Filename);

            return Save(filename);
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
        /// <param name="filename"></param>
        /// <returns></returns>
        private bool Save(string filename) {
            var success = false;

            if (filename != null) {
                lock (_locker) {
                    success = Serialize(filename);
                }
            }

            if (!success) {
                OnInfo("Failed to save configurable " + GetType().Name + " to " + (filename ?? "(unknown)") + ".");
            } else {
                lock (_locker) {
                    _isDirty = false;
                }

                OnInfo("Configurable " + GetType().Name + " saved to " + filename + ".");
            }

            return success;
        }

        #endregion
    }
}
