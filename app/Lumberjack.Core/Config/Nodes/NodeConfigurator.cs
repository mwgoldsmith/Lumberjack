using System.Collections.Generic;
using System.Xml.Serialization;

namespace Medidata.Lumberjack.Core.Config.Nodes
{
    public class NodeConfigurator : ConfigurableBase
    {
        #region Constants

        //private const string SettingsKey = "NodesFilename";
        private const string DefaultFilename = "nodes.xml";

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        public  NodeConfigurator() : this(null) { }

        /// <summary>
        /// 
        /// </summary>
        public NodeConfigurator(UserSession session) : base(session) {
            Nodes = new List<NodeElement>();
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
        [XmlElement("node", typeof(NodeElement))]
        public List<NodeElement> Nodes { set; get; }
        
        #endregion

        protected override void Initialize(ConfigurableBase config) {
            if (config != null) {
                Nodes = ((NodeConfigurator)config).Nodes;
            } else {
                Nodes.Clear();
            }
        }
    }
}
