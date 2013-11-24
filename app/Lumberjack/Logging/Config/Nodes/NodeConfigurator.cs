using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Medidata.Lumberjack.Logging.Config.Nodes
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlRoot("nodes")]
    public class NodeConfigurator : ConfigurableBase
    {          
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        public NodeConfigurator()
            : this(null)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public NodeConfigurator(string filename)
            : base(filename)
        {
            Nodes = new List<NodeElement>();
        }

        #endregion
    
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("node", typeof(NodeElement))]
        public List<NodeElement> Nodes { get; set; }

        #endregion

        #region Base overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        protected override void Initialize(ConfigurableBase config)
        {
            Nodes = ((NodeConfigurator)config).Nodes;
        }

        #endregion
    }
}
