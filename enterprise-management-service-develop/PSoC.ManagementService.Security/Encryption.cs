using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.WindowsAzure;

namespace PSoC.ManagementService.Security
{
    public sealed class Encryption : IDisposable
    {
        private static readonly SHA1 _sha1 = new SHA1CryptoServiceProvider();

        private static volatile Encryption instance;
        private static object syncRoot = new Object();

        private X509Certificate2 _certificate;

        #region Constructors

        private Encryption()
        {
            if (_certificate == null)
            {
                string thumbprint = CloudConfigurationManager.GetSetting("EncryptionCertificateThumbprint");

                if (string.IsNullOrWhiteSpace(thumbprint))
                {
                    throw new ApplicationException("Certificate thumbprint was not supplied.");
                }

                _certificate = new X509Certificate2();

                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);
                try
                {
                    bool onlyValidCerts = true;

#if (DEBUG || LOAD)
                    onlyValidCerts = false;
#endif

                    X509Certificate2Collection certCollection =
                      store.Certificates.Find(X509FindType.FindByThumbprint,
                                              thumbprint,
                                              onlyValidCerts);

                    if (certCollection.Count >= 1)
                    {
                        _certificate = certCollection[0];

                    }
                    else
                        throw new ApplicationException("Certificate not found in store.");
                }
                finally
                {
                    store.Close();
                }

            }
        }

        ~Encryption()
        {
            Dispose(true);
        }

        #endregion Constructors

        /// <summary>
        /// Singleton
        /// </summary>
        public static Encryption Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Encryption();
                    }
                }

                return instance;
            }
        }

        private X509Certificate2 Certificate
        {
            get
            {
                return _certificate;
            }
        }

        /// <summary>
        /// Encrypts the text one way.
        /// </summary>
        /// <param name="value">The value to hash</param>
        /// <returns>The hashed text.</returns>
        public byte[] ComputeHash(string value)
        {
            return ComputeHash(Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// Encrypts the byte array one way.
        /// </summary>
        /// <param name="value">The value to hash</param>
        /// <returns>The hashed byets.</returns>
        public byte[] ComputeHash(byte[] value)
        {
            byte[] result = _sha1.ComputeHash(value);

            return result;
        }

        /// <summary>
        /// Decrypt string value with RSA 256 x509 cert
        /// </summary>
        /// <param name="value">the value to decrypt</param>
        /// <returns>Decrypted string</returns>
        public string Decrypt(string value)
        {
            return Encoding.UTF8.GetString(DecryptToBytes(value));
        }

        /// <summary>
        /// Decrypt byte[] value with RSA 256 x509 cert
        /// </summary>
        /// <param name="value">the value to decrypt</param>
        /// <returns>Decrypted byte array</returns>
        public byte[] Decrypt(byte[] value)
        {
            byte[] decryptedData;
            var service = (RSACryptoServiceProvider)Certificate.PrivateKey;
            decryptedData = service.Decrypt(value, true);

            return decryptedData;
        }

        /// <summary>
        /// Decrypt string value with RSA 256 x509 cert
        /// </summary>
        /// <param name="value">the value to decrypt</param>
        /// <returns>Decrypted byte array</returns>
        public byte[] DecryptToBytes(string value)
        {
            var sb = Convert.FromBase64String(value);

            var decrBytes = Decrypt(sb);

            return decrBytes;
        }

        public void Dispose()
        {
            Dispose(false);
        }

        /// <summary>
        /// Encrypt string value with RSA 256 x509 cert
        /// </summary>
        /// <param name="value">the value to encrypt</param>
        /// <returns>Base64 encrypted string</returns>
        public string Encrypt(string value)
        {
            return Convert.ToBase64String(EncryptToBytes(value));
        }

        /// <summary>
        /// Encrypt byte[] value with RSA 256 x509 cert
        /// </summary>
        /// <param name="value">the value to encrypt</param>
        /// <returns>encrypted byte array</returns>
        public byte[] Encrypt(byte[] value)
        {
            byte[] encryptedData;
            var service = (RSACryptoServiceProvider)Certificate.PublicKey.Key;

            encryptedData = service.Encrypt(value, true);

            return encryptedData;
        }

        /// <summary>
        /// Encrypt string value with RSA 256 x509 cert
        /// </summary>
        /// <param name="value">the value to encrypt</param>
        /// <returns>encrypted byte array</returns>
        public byte[] EncryptToBytes(string value)
        {
            var sb = Encoding.UTF8.GetBytes(value);

            var encBytes = Encrypt(sb);

            return encBytes;
        }

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (_certificate != null)
                    {
                        _certificate.PrivateKey.Dispose();
                        _certificate.PublicKey.Key.Dispose();
                    }
                }
                catch (CryptographicException ex)
                {
                    //swallow it. I haven't found a way to test if
                    //_certificate.PrivateKey is empty without throwing this error
                }
                finally
                {
                    if (_certificate != null)
                    {
                        _certificate.Reset();
                        _certificate = null;
                    }

                    if (_sha1 != null)
                    {
                        _sha1.Dispose();
                    }

                }
            }
        }
    }
}