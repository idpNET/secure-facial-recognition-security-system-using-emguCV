
/*  Password hashing using PBKDF2 (SHA256 hashing Algorithm)
 *  +Setting iteration time
 *  +Hashing process time tracking
 *  https://github.com/idpNET/PBKDF2_hashing
 */

using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace PBKDF2_hashing
{
    /// <summary>
    /// PBKDF2 hashing using SHA256 Algorithm
    /// </summary>
    internal static class HashComputation
    {
        #region Variables Declaration 
        // Defines Hashing algorithm, number of iterations, and keys' size
        private const int SaltKeySize = 48;
        private const int HashKeySize = 128;
        private static int Iterations = 10000;
        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;
        #endregion

        #region Class Methods
        /// <summary>
        /// Computes hash value of an input byte[] array
        /// </summary>
        /// <param name="InputPassword"></param>
        /// <param name="InputSaltValue"></param>
        /// <remarks>This overload takes salt value as input via method parameter</remarks>
        /// <returns>Computed hash value in byte[]</returns>
        #region Hash Computation
        public static byte[] ComputeBytesHash(string InputPassword, byte[] InputSaltValue)
        {
            // Declares byte[] array which holds the computed hash value
            byte[] hashValue;

            // Computes and returning a hash value (specified KeySize) using System.Security.Cryptography.Rfc2898DeriveBytes
            // class using the input password, salt, number of iterations and the hash algorithm
            using (var deriveBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(InputPassword), InputSaltValue, Iterations, HashAlgorithm))
            {
                hashValue = deriveBytes.GetBytes(HashKeySize);
            }

            return hashValue;
        }

        /// <summary>
        /// Computes hash value of an input byte[] array
        /// </summary>
        /// <param name="InputPassword"></param>
        /// <param name="InputSaltValue"></param>
        /// <remarks>This overload doesn't takes salt value as input via method parameter</remarks>
        /// <returns>Computed hash value in byte[]</returns>
        public static byte[] ComputeBytesHash(string inputPassword, out byte[] Salt)
        {
            // Declares a byte array which holds the salt value
            Salt = new byte[SaltKeySize];

            // Instantiates a random number generator using RNGCryptoServiceProvider
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            // Fills the salt with cryptographically strong byte values.
            rng.GetNonZeroBytes(Salt);

            // Computes and returning a hash value (specified KeySize) using System.Security.Cryptography.Rfc2898DeriveBytes
            // class using the input password, salt, number of iterations and the hash algorithm
            byte[] hashValue;
            using (var deriveBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(inputPassword), Salt, Iterations, HashAlgorithm))
            {
                hashValue = deriveBytes.GetBytes(HashKeySize);
            }

            return hashValue;
        }
        #endregion

        /// <summary>
        /// Tracks hashing process time
        /// </summary>
        /// <param name="codeToExecute"></param>
        /// <returns>A read-only System.TimeSpan representing the total elapsed time measured by the current instance</returns>
        #region RunTimeMeasurement
        private static TimeSpan RunTimeMeasurement(Action codeToExecute)
        {
            //  Starts time tracking, initializing a new System.Diagnostics.Stopwatch instance
            var watch = Stopwatch.StartNew();
            // Action execution flow
            codeToExecute();
            // Stops time tracking
            watch.Stop();
            return watch.Elapsed;
        }
        #endregion
        #endregion

    }
}

