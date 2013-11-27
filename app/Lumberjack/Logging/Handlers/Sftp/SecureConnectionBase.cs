using System;
using System.IO;
using Medidata.Lumberjack.Properties;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace Medidata.Lumberjack.Logging.Handlers.Sftp
{
    public abstract class SecureConnectionBase : SessionComponentBase, IDisposable
    {
        #region Private fields

        private PrivateKeyFile _keyFile;
        protected BaseClient _client = null;

        #endregion

        #region Initializers

        /// <summary>
        /// Create a new instance of SecureConnectionBase.
        /// </summary>
        /// <param name="session">The Session which the connection is to be used in</param>
        protected SecureConnectionBase(Session session)
            : base(session)
        {
            ErrorType = SecureErrorTypeEnum.None;
            IsConnected = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// True if the secure connection is currently connected; otherwise, false.
        /// </summary>
        public bool IsConnected { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public SecureErrorTypeEnum ErrorType { get; protected set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Load the private key, which must be in OpenSSH format. Once the key is loaded, it does not need
        /// to be loaded again for the lifetime of the application.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="passphraseAes"></param>
        /// <returns>True if successful; otherwise, false.</returns>
        public bool LoadPrivateKey(string filename, string passphraseAes)
        {
            var crypto = new CryptoAes();

            // Dispose of the key if it was already loaded
            if (_keyFile != null)
            {
                _keyFile.Dispose();
                _keyFile = null;
            }

            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                try
                {
                    _keyFile = new PrivateKeyFile(fs, crypto.DecryptString(passphraseAes));
                }
                catch (Exception ex)
                {
                    ErrorMessage(ex);
                    throw;
                }

            return _keyFile != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected ConnectionInfo GetConnectionInfo(string host, int port)
        {
            var crypto = new CryptoAes();
            ConnectionInfo connection = null;

            if (_keyFile == null)
            {
                LoadPrivateKey(Settings.Default.SshKeyFilename, Settings.Default.SshPassphraseAes);
            }

            if (_keyFile == null)
            {
                ErrorType = SecureErrorTypeEnum.UnreadableKey;
                ErrorMessage("Failed to load private key");
            }
            else
            {
                var username = crypto.DecryptString(Settings.Default.SshUsernameAes);
                connection = new ConnectionInfo(host, port, username, new PrivateKeyAuthenticationMethod(username, _keyFile));
                ErrorType = SecureErrorTypeEnum.None;
            }

            return connection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if successful; otherwise, false.</returns>
        protected bool ConnectToHost(BaseClient client)
        {
            var success = false;

            if (client == null)
            {
                IsConnected = false;
                return false;
            }

            try
            {
                _client = client;
                _client.ErrorOccurred += OnErrorOccurred;
                _client.Connect();

                if (!(success = _client.IsConnected))
                    ErrorMessage("Failed to connect to host");
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                ErrorType = SecureErrorTypeEnum.SocketFailed;
                ErrorMessage("Failed to connect to host. Reason: " + ex.SocketErrorCode);
            }
            catch (SshAuthenticationException ex)
            {
                ErrorType = SecureErrorTypeEnum.AuthenticationFailed;
                ErrorMessage("Authentication failed connecting to '" + _client.ConnectionInfo.Host + "'. Reason: " + ex.Message);
            }
            catch (Exception ex)
            {
                ErrorType = SecureErrorTypeEnum.UnknownException;
                ErrorMessage(ex);
            }

            if (!success)
            {
                if (_client.IsConnected) _client.Disconnect();

                _client.ErrorOccurred -= OnErrorOccurred;
                _client.Dispose();
                _client = null;
            }

            IsConnected = success;

            return success;
        }

        /// <summary>
        /// Close a previously opened connection
        /// </summary>
        /// <returns>True if successful; otherwise, false.</returns>
        public virtual bool Close()
        {
            if (_client != null)
            {
                _client.Disconnect();
                _client.ErrorOccurred -= OnErrorOccurred;
                _client.Dispose();
                _client = null;
            }

            IsConnected = false;

            return true;
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnErrorOccurred(object sender, ExceptionEventArgs e)
        {

        }

        #endregion

        #region IDisposable Members

        private bool _isDisposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (_isDisposed) return;

            // Release unmanaged resources
            // (none)

            if (disposing)
            {
                // Release managed resources
                if (_client != null)
                {
                    _client.Disconnect();
                    _client.ErrorOccurred -= OnErrorOccurred;
                    _client.Dispose();
                    _client = null;
                }
            }

            // Note disposing has been done.
            _isDisposed = true;
        }

        #endregion
    }
}
