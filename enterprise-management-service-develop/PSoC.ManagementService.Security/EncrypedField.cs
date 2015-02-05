using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSoC.ManagementService.Security
{
    public class EncrypedField<T>
    {
        #region Constructors

        public EncrypedField()
        {
        }

        /// <summary>
        /// Constructor using encrypted vsalue
        /// </summary>
        /// <param name="encryptedValue">the encrypted value</param>
        public EncrypedField(string encryptedValue)
        {
            this.DecryptedValue = Encryption.Instance.Decrypt(encryptedValue);
            this.EncryptedValue = Convert.FromBase64String(encryptedValue);
        }

        public EncrypedField(byte[] encryptedValue)
        {
            this.DecryptedValue = Encoding.UTF8.GetString(Encryption.Instance.Decrypt(encryptedValue));
            this.EncryptedValue = encryptedValue;
        }

        #endregion Constructors

        /// <summary>
        /// Decrypted Value for field
        /// </summary>
        public string DecryptedValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Encrypted value for field
        /// </summary>
        public byte[] EncryptedValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Implicit operator to set a value to the class from T
        /// </summary>
        /// <param name="value">The value to set to the class</param>
        /// <returns></returns>
        public static implicit operator EncrypedField<T>(T value)
        {
            if (value == null)
                return null;

            if (value.GetType() == typeof(string))
            {
                var val = value as string;
                return new EncrypedField<T>
                {
                    DecryptedValue = val,
                    EncryptedValue = Encryption.Instance.EncryptToBytes(val)
                };
            }
            else if (value.GetType() == typeof(byte[]))
            {
                var val = value as byte[];
                return new EncrypedField<T>
                {
                    DecryptedValue = Encoding.UTF8.GetString(val),
                    EncryptedValue = Encryption.Instance.Encrypt(val)
                };
            }
            else
                throw new TypeAccessException(string.Format("Unsupported type.  Type: {0} was supplied. Only string and byte[] are supported.", value.GetType()));
        }

        /// <summary>
        /// Implicit operator to get the value of the class as a string.
        /// </summary>
        /// <param name="value">The instance of the class</param>
        /// <returns>an <c>string</c> value for the class</returns>
        public static implicit operator string(EncrypedField<T> value)
        {
            return value != null ? value.ToString() : string.Empty;
        }

        ///<summary>
        /// Implicit operator to get the value of the class as a byte array.
        /// </summary>
        /// <param name="value">The instance of the class</param>
        /// <returns>a <c>byte[]</c> value for the class; otherwise null</returns>
        public static implicit operator byte[](EncrypedField<T> value)
        {
            return value != null ? value.EncryptedValue as byte[] : null;
        }

        /// <summary>
        /// Gets the Sha1 hash for the decrypted value
        /// </summary>
        /// <returns>the Base64 hash string</returns>
        public string GetHashString()
        {
            var hashCode = GetHashBytes();

            return Convert.ToBase64String(hashCode);
        }

        /// <summary>
        /// Gets the Sha1 hash for the decrypted value
        /// </summary>
        /// <returns>the binary hash</returns>
        public byte[] GetHashBytes()
        {
            var hashCode = Encryption.Instance.ComputeHash(this.DecryptedValue as string);

            return hashCode;
        }

        /// <summary>
        /// Returns the Value field as a string.
        /// </summary>
        /// <returns>string value of the Value field</returns>
        public override string ToString()
        {
            if (EncryptedValue != null)
                return Encoding.UTF8.GetString(EncryptedValue);

            return string.Empty;
        }
    }
}