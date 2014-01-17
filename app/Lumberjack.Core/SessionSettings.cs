namespace Medidata.Lumberjack.Core
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SessionSettings
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        public SessionSettings()
            : this(null) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public SessionSettings(SessionSettings settings) {
            if (settings == null)
                return;

            FieldConfigFile = settings.FieldConfigFile;
            FormatConfigFile = settings.FormatConfigFile;
            NodeConfigFile = settings.NodeConfigFile;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string FieldConfigFile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FormatConfigFile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string NodeConfigFile { get; set; }

        #endregion
    }
}
