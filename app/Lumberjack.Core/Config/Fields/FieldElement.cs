using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Medidata.Lumberjack.Core.Data.Fields;

namespace Medidata.Lumberjack.Core.Config.Fields
{
    /// <summary>
    /// A FieldElement object is used as the base definition for a Field, solely for
    /// the purpose of serialization. It provides the structure most suitable for
    /// layout as XML, and is used as an intermediary between the XML file and session
    /// Field objects.
    /// </summary>
    [Serializable]
    [XmlType("FieldElement")]
    public class FieldElement : FieldBase
    {    
        #region Initializers

        /// <summary>
        /// Creates a new FieldElement instance.
        /// </summary>
        public FieldElement() {
            FieldContexts = new List<FieldContextElement>();
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// If true, the field can be filtered by value; otherwise it cannot be filtered.
        /// To allow the field to be filtered, the value will be retrieved when the log 
        /// is parsed and it will be stored in the session field value cache. 
        /// </summary>
        [XmlAttribute("filterable", DataType = "boolean")]
        public override bool Filterable {
            get { return base.Filterable; }
            set { base.Filterable = value; }
        }

        /// <summary>
        /// Array of the format types in which the field is permitted.
        /// </summary>
        [XmlElement("context", typeof(FieldContextElement))]
        public List<FieldContextElement> FieldContexts { set; get; }

        #endregion
    }
}
