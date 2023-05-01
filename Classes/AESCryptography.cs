using System.IO;
using System.Security.Cryptography;

namespace Secure_Facial_Recognition_Security_System_using_EmguCV
{
    /// <summary>
    /// Encrypts and Decrypts Byte[] array using AES cryptography
    /// </summary>
    internal static class AESCryptography
    {

        #region Variables Declaration 
        // Defines KEY and IV values used in AES cryptography
        private static byte[] KEY = { 145, 12, 32, 245, 98, 132, 98, 214, 6, 77, 131, 44, 221, 3, 9, 50 };
        private static byte[] IV = { 15, 122, 132, 5, 93, 198, 44, 31, 9, 39, 241, 49, 250, 188, 80, 7 };
        #endregion

        #region Class Methods
        /// <summary>
        /// Encrypts input byte[] value using AES cryptography
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Encrypted byte[]</returns>
        #region Encryption
        public static byte[] Encrypt(byte[] data)
        {
            using (Aes algorithm = Aes.Create())
            using (ICryptoTransform encryptor = algorithm.CreateEncryptor(KEY, IV))
                return Crypt(data, encryptor);
        }
        #endregion

        /// <summary>
        /// Decrypts input byte[] value using AES cryptography
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Decrypted byte[]</returns>
        #region Decryption
        public static byte[] Decrypt(byte[] data)
        {
            using (Aes algorithm = Aes.Create())
            using (ICryptoTransform decryptor = algorithm.CreateDecryptor(KEY, IV))
                return Crypt(data, decryptor);
        }
        #endregion

        /// <summary>
        /// Cryptographic transformation using CryptoStream
        /// </summary>
        /// <returns>byte[]</returns>
        #region Cryptographic transformation
        private static byte[] Crypt(byte[] data, ICryptoTransform cryptor)
        {
            MemoryStream m = new MemoryStream();
            using (Stream c = new CryptoStream(m, cryptor, CryptoStreamMode.Write))
                c.Write(data, 0, data.Length);
            return m.ToArray();
        }
        #endregion
        #endregion
    }
}
