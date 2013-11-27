using System;
using Renci.SshNet;

namespace Medidata.Lumberjack.Logging.Handlers.Sftp
{
    public class SftpConnection : SecureConnectionBase
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public SftpConnection(Session session) 
            : base(session)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public SftpClient Client
        {
            get { return _client as SftpClient; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Open an connection via SFTP to a site. The private key must have been previously loaded
        /// or able to be loaded at this time.
        /// </summary>
        /// <returns>True if successful; otherwise, false.</returns>
        public bool Open(string host, int port)
        {
            var success = false;

            try
            {
                SftpClient client = null;

                var connection = GetConnectionInfo(host, port);
                if (connection != null)
                    client = new SftpClient(connection);

                success = client != null;
                if (success)
                    success = ConnectToHost(client);
            }
            catch (Exception ex)
            {
                base.Close();
                ErrorMessage(ex);
            }
            
            return success;
        }

        #endregion
    }
}
