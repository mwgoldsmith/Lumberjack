using System;
using System.Collections.Generic;
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
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        public FormatConfigurator()
            : this(null)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public FormatConfigurator(string filename)
            : base(filename)
        {
            Formats = new List<LogFormat>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("format", typeof(LogFormat))]
        public List<LogFormat> Formats { get; set; }

        #endregion

        #region Base overrides

        protected override void Initialize(ConfigurableBase config)
        {
            Formats = ((FormatConfigurator)config).Formats;
        }

        #endregion
    }
}
