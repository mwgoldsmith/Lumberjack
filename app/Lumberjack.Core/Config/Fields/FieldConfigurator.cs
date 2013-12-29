using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Medidata.Lumberjack.Core.Config.Fields
{    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlRoot("fields")]
    public class FieldConfigurator : ConfigurableBase
    {
        #region Constants

        private const string DefaultFilename = "fields.xml";

        #endregion

        #region Initializers

        /// <summary>
        /// Creates a new FieldConfigurator instance.
        /// </summary>
        public FieldConfigurator() : this(null) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public FieldConfigurator(UserSession session) :base(session) {
            Fields = new List<FieldElement>();
            base.Filename = DefaultFilename;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        protected override string Filename {
            get {
                return base.Filename;
                //return Util.GetSettingOrDefault(SettingsKey);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("field", typeof(FieldElement))]
        public List<FieldElement> Fields { get; set; }

        #endregion

        #region Base overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        protected override void Initialize(ConfigurableBase config) {
            if (config != null) {
                Fields = ((FieldConfigurator)config).Fields;
            } else {
                Fields.Clear();
            }
        }

        #endregion
    }
}
