using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Medidata.Lumberjack.Core.Data.Formats;

namespace Medidata.Lumberjack.Core.Config.Formats
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlRoot("formats")]
    public class FormatConfigurator : ConfigurableBase
    {
        #region Constants

        //private const string SettingsKey = "FormatsFilename";
        private const string DefaultFilename = "formats.xml";
        #endregion

        #region Initializers

        /// <summary>
        /// Creates a new FormatConfigurator instance.
        /// </summary>
        public FormatConfigurator():this(null) {
        }

        /// <summary>
        /// Creates a new FormatConfigurator instance.
        /// </summary>
        public FormatConfigurator(UserSession session) :base(session) {
            Formats = new List<FormatElement>();
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
        [XmlElement("format", typeof (FormatElement))]
        public List<FormatElement> Formats { get; set; }

        #endregion

        #region Base overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        protected override void Initialize(ConfigurableBase config) {
            if (config != null) {
                Formats = ((FormatConfigurator) config).Formats;

                foreach (var f in Formats) {
                    SetFormatFieldContextType(f.FilenameContext, FormatContextEnum.Filename);
                    SetFormatFieldContextType(f.EntryContext, FormatContextEnum.Entry);
                    SetFormatFieldContextType(f.ContentContext, FormatContextEnum.Content);
                }
            } else {
                Formats.Clear();
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatContextElement"></param>
        /// <param name="context"></param>
        private void SetFormatFieldContextType(FormatContextElement formatContextElement, FormatContextEnum context) {
            if (formatContextElement == null)
                return;

            foreach (var f in formatContextElement.Fields)
                f.Type = context;
        }

        #endregion
    }
}
