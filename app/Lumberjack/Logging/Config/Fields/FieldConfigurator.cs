using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Medidata.Lumberjack.Logging.Config.Fields
{
    [Serializable]
    [XmlRoot("fields")]
    public class FieldConfigurator : ConfigurableBase
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        public FieldConfigurator()
            : this(null)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public FieldConfigurator(string filename)
            : base(filename)
        {
            Fields = new List<FieldElement>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("field", typeof(FieldElement))]
        public List<FieldElement> Fields { get; set; }

        #endregion

        #region Base overrides

        protected override void Initialize(ConfigurableBase config)
        {
            Fields = ((FieldConfigurator)config).Fields;
        }

        #endregion
    }
}
