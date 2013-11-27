using System;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Medidata.Lumberjack.Logging.Config.Formats
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlType("Field")]
    public class FieldElement
    {
        #region Private fields

        private string _indexString;
        private Int32[] _indexArray;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("id", DataType = "string")]
        public string Identifier { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("required", DataType = "boolean")]
        public bool Required { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("default", DataType = "string")]
        public string Default { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlText]
        public string GroupIndexes
        {
            get { return _indexString; }
            set {
                _indexString = value;
                _indexArray = null;
            }
        }
        
        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Int32[] GetGroups()
        {
            if (_indexArray == null && !String.IsNullOrEmpty(_indexString))
            {
                var indexes = _indexString.Split(',').ToList();

                _indexArray = new Int32[indexes.Count];
                for (var i = 0; i < indexes.Count; i++)
                    _indexArray[i] = Int32.Parse(indexes[i], NumberStyles.Integer);
            }

            return _indexArray;
        }

        #endregion
    }
}
