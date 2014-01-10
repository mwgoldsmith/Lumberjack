using System;
using System.Diagnostics;
using Medidata.Lumberjack.Core.Config.Formats;
using Medidata.Lumberjack.Core.Data.Collections;

namespace Medidata.Lumberjack.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public sealed class SessionFormat
    {
        #region Private fields

        // Saving reference to objects from which the values of this SessionFormat
        // were derived. Not sure if will be needed later, but keeping them for now
        private readonly FormatElement _formatElement;
        private readonly UserSession _session;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="formatElement"></param>
        /// <param name="id"></param>
        public SessionFormat(UserSession session, FormatElement formatElement, byte id) {
            _formatElement = formatElement;
            _session = session;

            Id = id;
            Reference = formatElement.Reference;
            Name = formatElement.Name;
            TimestampFormat = formatElement.TimestampFormat;

            Contexts = new ContextCollection(session);
            Contexts[FormatContextEnum.Filename] = new ContextFormat(session, this, formatElement.FilenameContext);
            Contexts[FormatContextEnum.Entry] = new ContextFormat(session, this, formatElement.EntryContext);
            Contexts[FormatContextEnum.Content] = new ContextFormat(session, this, formatElement.ContentContext);

            // TODO: Verify that for each FormatField within each ContextFormat:
            //   - if a given SessionField (referred to by the FormatField) is marked as Required,
            //     there is a FormatField set as Required. Exception is thrown if more than one (or
            //     there aren't any) set as Required
            //   - if a given SessionField (referred to by the FormatField) is NOT marked as Required,
            //     and it IS set as Required in more than one context, throw exception.
            //   - Verify that more than one FormatField referring to the same SessionField do
            //     no exist within the same context
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public byte Id { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Reference { get; private set; }

        /// <summary>
        ///  
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string TimestampFormat { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ContextCollection Contexts { get; private set; }

        #endregion

        #region ToString override for debugging

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return String.Format("{{ " +
                "Id = {0}, " +
                "Reference = {1}, " +
                "Name = {2}, " +
                "TimestampFormat = {3}, " +
                "Contexts = {4} }}",
                Id,
                Reference,
                Name,
                TimestampFormat,
                Contexts);
        }

        #endregion
    }
}