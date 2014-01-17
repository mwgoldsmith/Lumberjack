using System;
using System.Text.RegularExpressions;
using Medidata.Lumberjack.Core.Collections;
using Medidata.Lumberjack.Core.Config.Formats;
using Medidata.Lumberjack.Core.Data.Fields;
using Medidata.Lumberjack.Core.Logging;

namespace Medidata.Lumberjack.Core.Data.Formats
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ContextFormat
    {
        #region Private fields

        private readonly FormatContextElement _formatContextElement;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="sessionFormat"></param>
        /// <param name="formatContextElement"></param>
        public ContextFormat(UserSession session, SessionFormat sessionFormat, FormatContextElement formatContextElement) {
            var formatFields = session.FormatFields;

            _formatContextElement = formatContextElement;
            Regex = CreateRegex(formatContextElement.RegexElement);
            Fields = new FormatFieldCollection(session);
            // TODO: make this set to a wrapped instance of the session's FormatFieldCollection, but 
            // TODO: restricted to the items for this object


            foreach (var field in formatContextElement.Fields) {
                var sessionField = session.SessionFields.Find(field.Name);
                var formatField = new FormatField(sessionFormat, field, sessionField);

                formatFields.Add(formatField);
                Fields.Add(formatField);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Regex Regex { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public FormatFieldCollection Fields { get; private set; }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="regexElement"></param>
        /// <returns></returns>
        private static Regex CreateRegex(RegexElement regexElement) {
            if (regexElement == null)
                throw new ArgumentNullException("regexElement");

            var pattern = regexElement.Pattern;
            if (String.IsNullOrEmpty(pattern)) {
                Logger.DefaultLogger.Error("Regex pattern not provided in RegexElement element!");
                return null;
            }

            // Determine options to use for Regex instance
            var opts = RegexOptions.Compiled | RegexOptions.CultureInvariant
                       | (regexElement.CaseInsensitive ? RegexOptions.IgnoreCase : 0)
                       | (regexElement.Singleline ? RegexOptions.Singleline : 0)
                       | (regexElement.Multiline ? RegexOptions.Multiline : 0);

            return new Regex(pattern, opts);
        }

        #endregion
 
        #region ToString override for debugging

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return String.Format("{{ " +
                                 "Regex = {0}, " +
                                 "Fields = {1} }}",
                                 _formatContextElement.RegexElement.Pattern,
                                 Fields);
        }

        #endregion
    }
}
