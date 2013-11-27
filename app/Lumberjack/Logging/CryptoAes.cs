// Medidata.Lumberjack.Logging.CryptoAes
//
// Author:      Michael Goldsmith
// Last Update: 2013-MAR-11
//
// Comments:    Modified version. Originaly by Mark Brittingham
//

using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace Medidata.Lumberjack.Logging
{
    /// <summary>
    /// Provides encryption and decryption of information which could pose a potential security-risk
    /// </summary>
    public class CryptoAes
    {
        #region Private fields

        private readonly byte[] _key = { 123, 207, 19, 11, 24, 26, 85, 45, 114, 184, 27, 162, 37, 112, 222, 219, 241, 24, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53, 209 };
        private readonly byte[] _vector = { 146, 64, 191, 111, 23, 3, 113, 129, 231, 121, 221, 112, 79, 32, 114, 156 };

        private readonly ICryptoTransform _encryptorTransform;
        private readonly ICryptoTransform _decryptorTransform;
        private readonly UTF8Encoding _utfEncoder;

        #endregion

        #region Initializers

        /// <summary>
        /// Create a new instance of the CryptoAES class
        /// </summary>
        public CryptoAes()
        {
            var rm = new RijndaelManaged();

            _encryptorTransform = rm.CreateEncryptor(_key, _vector);
            _decryptorTransform = rm.CreateDecryptor(_key, _vector);

            _utfEncoder = new UTF8Encoding();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Generates a new encryption key (currently not used).
        /// </summary>
        /// <returns>The generated encryption key.</returns>
        public static byte[] GenerateEncryptionKey()
        {
            var rm = new RijndaelManaged();
            rm.GenerateKey();
            return rm.Key;
        }

        /// <summary>
        /// Generates a new encryption vector (currently not used).
        /// </summary>
        /// <returns>The generated encryption vector.</returns>
        public static byte[] GenerateEncryptionVector()
        {
            var rm = new RijndaelManaged();
            rm.GenerateIV();
            return rm.IV;
        }

        /// <summary>
        /// Encrypt a given string value.
        /// </summary>
        /// <param name="input">The string value to encrypt.</param>
        /// <returns>The encrypted string.</returns>
        public string EncryptToString(string input)
        {
            var bytes = EncryptToBytes(input);
            string encrypted;

            try
            {
                encrypted = Convert.ToBase64String(bytes);
            }
            catch
            {
                return null;
            }

            return encrypted;
        }

        /// <summary>
        /// Encrypt a given string value.
        /// </summary>
        /// <param name="input">The string value to encrypt.</param>
        /// <returns>The string encrypted as a byte array.</returns>
        public byte[] EncryptToBytes(string input)
        {
            Byte[] bytes = _utfEncoder.GetBytes(input);
            byte[] encrypted;

            using (var ms = new MemoryStream())
            {
                var cs = new CryptoStream(ms, _encryptorTransform, CryptoStreamMode.Write);
                
                cs.Write(bytes, 0, bytes.Length);
                cs.FlushFinalBlock();

                ms.Position = 0;
                encrypted = new byte[ms.Length];
                ms.Read(encrypted, 0, encrypted.Length);

                cs.Close();
                ms.Close();
            }

            return encrypted;
        }

        /// <summary>
        /// Decrypt a given string value.
        /// </summary>
        /// <param name="encrypted">The encrypted value as a string.</param>
        /// <returns>The decrypted string if successful; otherwise null.</returns>
        public string DecryptString(string encrypted)
        {
            if (string.IsNullOrEmpty(encrypted)) return null;

            byte[] byteArr;

            try
            {
                byteArr = Convert.FromBase64String(encrypted);
            }
            catch
            {
                return null;
            }

            return DecryptBytes(byteArr);
        }
  
        /// <summary>
        /// Decrypt a given byte array.
        /// </summary>
        /// <param name="encrypted">The encrypted value as a byte array.</param>
        /// <returns>The decrypted string.</returns>
        public string DecryptBytes(byte[] encrypted)
        {
            Byte[] decrypted;

            using (var es = new MemoryStream())
            {
                var ds = new CryptoStream(es, _decryptorTransform, CryptoStreamMode.Write);
                
                ds.Write(encrypted, 0, encrypted.Length);
                ds.FlushFinalBlock();

                es.Position = 0;
                decrypted = new Byte[es.Length];
                es.Read(decrypted, 0, decrypted.Length);

                ds.Close();
            }

            return _utfEncoder.GetString(decrypted);
        }
        
        #endregion
    }
}
