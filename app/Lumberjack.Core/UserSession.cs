using System;
using Medidata.Lumberjack.Core.Config.Fields;
using Medidata.Lumberjack.Core.Config.Formats;
using Medidata.Lumberjack.Core.Config.Nodes;
using Medidata.Lumberjack.Core.Data;
using Medidata.Lumberjack.Core.Data.Collections;
using Medidata.Lumberjack.Core.Logging;
using Medidata.Lumberjack.Core.Processing;

namespace Medidata.Lumberjack.Core
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class UserSession : SerializableBase, IDisposable
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        internal UserSession(SessionSettings settings) : base(null) {
            Settings = new SessionSettings(settings);

            SessionFields = new SessionFieldCollection(this);
            FieldValues = new FieldValueCollection(this);
            FieldConfig = new FieldConfigurator(this);

            Formats = new SessionFormatCollection(this);
            FormatFields = new FormatFieldCollection(this);
            FormatConfig = new FormatConfigurator(this);

            LogFiles = new LogFileCollection(this);
            Entries = new EntryCollection(this);

            NodeConfig = new NodeConfigurator(this);

            ProcessController = new ProcessController(this);
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// 
        /// </summary>
        public event MessageEventHandler Message;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public EntryCollection Entries { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public FieldConfigurator FieldConfig { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        public FieldValueCollection FieldValues { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public SessionFieldCollection SessionFields { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public FormatFieldCollection FormatFields { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FormatConfigurator FormatConfig { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public SessionFormatCollection Formats { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string LogDirectory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LogFileCollection LogFiles { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public NodeConfigurator NodeConfig { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ProcessController ProcessController { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public SessionSettings Settings { get; private set; }

        #endregion

        #region ?Internal methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void InvokeMessage(object sender, MessageEventArgs e) {
            if (Message != null) {
                Message.Invoke(this, e);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        public void LoadConfig() {
            LoadConfig(FieldConfig);
            LoadConfig(FormatConfig);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurator"></param>
        private void LoadConfig(FieldConfigurator configurator) {
            // Cannot load fields while log files present in the session
            if (!LogFiles.IsEmpty) {
                throw new NotSupportedException();
            }

            // If configurator has not yet initialized, load data
            if (!configurator.IsConfigured) {
                if (!configurator.Load())
                    return;
            }

            SessionFields.Clear();

            // Add each element within the configurator to the session field collection
            byte id = 0;
            foreach (var fieldElement in configurator.Fields) {
                SessionFields.Add(new SessionField(fieldElement, id++));
            }

            OnInfo("Loaded " + SessionFields.Count + " fields into user session.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurator"></param>
        private void LoadConfig(FormatConfigurator configurator) {
            // Verify fields have been loaded (formats are dependant)
            if (SessionFields.IsEmpty) {
                throw new InvalidOperationException("Cannot load formats before dependent data: no fields in session");
            }

            // Cannot load formats while log files present in the session
            if (!LogFiles.IsEmpty) {
                throw new NotSupportedException();
            }

            // If configurator has not yet initialized, load data
            if (!configurator.IsConfigured) {
                if (!configurator.Load())
                    return;
            }

            Formats.Clear();

            // Add each element within the configurator to the session format collection
            byte id = 0;
            foreach (var formatElement in configurator.Formats) 
                Formats.Add(new SessionFormat(this, formatElement, id++));
            
            OnInfo("Loaded " + Formats.Count + " formats into user session.");
        }
        
        #endregion

        #region IDisposable implementation

        /// <summary>
        /// 
        /// </summary>
        private bool IsDisposed { get; set; }

        /// <summary>
        /// Implementation of Dispose according to .NET Framework Design Guidelines.
        /// </summary>
        /// <remarks>
        /// Do not make this method virtual. A derived class should not be able to
        /// override this method.
        /// </remarks>
        public void Dispose() {
            Dispose(true);
        }

        /// <summary>
        /// Overloaded Implementation of Dispose.
        /// </summary>
        /// <param name="isDisposing"></param>
        /// <remarks>
        /// <para><list type="bulleted">Dispose(bool isDisposing) executes in two distinct scenarios.
        /// <item>If <paramref name="isDisposing"/> equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.</item>
        /// <item>If <paramref name="isDisposing"/> equals false, the method has been called by the 
        /// runtime from inside the finalizer and you should not reference 
        /// other objects. Only unmanaged resources can be disposed.</item></list></para>
        /// </remarks>
        private void Dispose(bool isDisposing) {
            try {
                if (IsDisposed)
                    return;

                if (isDisposing) {
                    if (ProcessController != null) {
                        ProcessController.Dispose();
                    }
                }

                ProcessController = null;
            } finally {
                IsDisposed = true;
            }
        }

        #endregion
    }
}
