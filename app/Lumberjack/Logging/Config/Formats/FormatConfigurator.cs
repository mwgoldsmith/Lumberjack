using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Medidata.Lumberjack.Logging.Config.Formats
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlRoot("formats")]
    public class FormatConfigurator : ConfigurableBase
    {
        #region Private fields

        private List<LogFormat> _formats;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        public FormatConfigurator()
            : this("FormatsFilename")
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public FormatConfigurator(string settingsKey)
            : base(settingsKey)
        {
            _formats = new List<LogFormat>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("format", typeof(LogFormat))]
        public List<LogFormat> Formats
        {
            get { return _formats; }
            set { _formats = value; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Retreives a list of the parsers within the session. NOTE: The return value is a shallow
        /// clone and not a reference to the actual list used internally. As such, modifying the
        /// list will not affect the logs within the session. However, the Parser items reference
        /// the same object and CAUTION should be used when accessing from multiple threads.
        /// </summary>
        /// <returns></returns>
        public List<LogFormat> GetFormats()
        {
            var formats = new List<LogFormat>();

            lock (_locker)
            {
                if (!IsConfigured) Load();

                formats.AddRange(_formats);
            }

            return formats;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public Dictionary<string, FormatBase> FindAll(FormatTypeEnum formatType)
        {
            var formats = new Dictionary<string, FormatBase>();
      
            lock (_locker)
            {
                if (!IsConfigured) Load();

                foreach (var f in _formats)
                {
                    if (formatType == FormatTypeEnum.Filename && f.Filename != null)
                        formats.Add(f.Identifier, f.Filename);
                    else if (formatType == FormatTypeEnum.Entry && f.Entry != null)
                        formats.Add(f.Identifier, f.Entry);
                    else if (formatType == FormatTypeEnum.Content && f.Content != null)
                        formats.Add(f.Identifier, f.Content);
                }
            }

            return formats;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public LogFormat Find(string identifier)
        {
            LogFormat format;

            lock (_locker)
            {
                if (!IsConfigured) Load();

                format = _formats.FirstOrDefault(f =>
                    string.Equals(f.Identifier, identifier, StringComparison.InvariantCultureIgnoreCase));

            }

            return format;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public FormatBase Find(string identifier, FormatTypeEnum formatType)
        {
            LogFormat format;

            lock (_locker)
            {
                if (!IsConfigured) Load();

                format = _formats.FirstOrDefault(f =>
                    string.Equals(f.Identifier, identifier, StringComparison.InvariantCultureIgnoreCase));
            }

            if (format == null) return null;
            if (formatType == FormatTypeEnum.Filename) return format.Filename;
            if (formatType == FormatTypeEnum.Entry) return format.Entry;
            return formatType == FormatTypeEnum.Content ? format.Content : null;
        }

        #endregion

        #region Base overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        protected override void Initialize(ConfigurableBase config)
        {
            Formats = ((FormatConfigurator)config).Formats;
        }

        #endregion
    }
}
