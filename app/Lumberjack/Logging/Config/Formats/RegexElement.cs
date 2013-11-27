using System;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Medidata.Lumberjack.Logging.Config.Formats
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlType("ParserRegex")]
    public class RegexElement
    {
        #region Private fields

        private Regex _regex;
        private bool _caseInsensitive;
        private bool _singleline;
        private bool _multiline;
        private string _pattern;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("caseInsensitive", DataType = "boolean")]
        public bool CaseInsensitive
        {
            get { return _caseInsensitive; }
            set { 
                _caseInsensitive = value;
                _regex = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("singleline", DataType = "boolean")]
        public bool Singleline
        {
            get { return _singleline; }
            set
            {
                _singleline = value;
                _regex = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("multiline", DataType = "boolean")]
        public bool Multiline
        {
            get { return _multiline; }
            set
            {
                _multiline = value;
                _regex = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlText]
        public string Pattern
        {
            get { return _pattern; }
            set
            {
                _pattern = value;
                _regex = null;
            }
        }
        
        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Regex GetExpression()
        {
            // Construct the compiled RegEx if we haven't already
            if (_regex == null && !String.IsNullOrWhiteSpace(_pattern))
            {
                var opts = RegexOptions.Compiled | RegexOptions.CultureInvariant
                           | (_caseInsensitive ? RegexOptions.IgnoreCase : 0)
                           | (_singleline ? RegexOptions.Singleline : 0)
                           | (_multiline ? RegexOptions.Multiline : 0);

                _regex = new Regex(_pattern, opts);
            }

            return _regex;
        }

        #endregion
    }
}
