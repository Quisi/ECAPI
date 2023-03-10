using System;
using System.Linq;
using System.Security.Cryptography;

namespace EnergyScanApi.Security
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; // 128 bit 
        private const int KeySize = 32; // 256 bit
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public PasswordHasher(HashingOptions options)
        {
            Options = options;
        }

        private HashingOptions Options { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string Hash(string password)
        {
            using (Rfc2898DeriveBytes algorithm = new Rfc2898DeriveBytes(
              password,
              SaltSize,
              Options.Iterations,
              HashAlgorithmName.SHA512))
            {
                string key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
                string salt = Convert.ToBase64String(algorithm.Salt);

                return $"{Options.Iterations}.{salt}.{key}";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public (bool Verified, bool NeedsUpgrade) Check(string hash, string password)
        {
            string[] parts = hash.Split('.', 3);

            if (parts.Length != 3)
            {
                throw new FormatException("Unexpected hash format. " +
                  "Should be formatted as `{iterations}.{salt}.{hash}`");
            }

            int iterations = Convert.ToInt32(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] key = Convert.FromBase64String(parts[2]);

            bool needsUpgrade = iterations != Options.Iterations;

            using (Rfc2898DeriveBytes algorithm = new Rfc2898DeriveBytes(
              password,
              salt,
              iterations,
              HashAlgorithmName.SHA512))
            {
                byte[] keyToCheck = algorithm.GetBytes(KeySize);

                bool verified = keyToCheck.SequenceEqual(key);

                return (verified, needsUpgrade);
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        string Hash(string password);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        (bool Verified, bool NeedsUpgrade) Check(string hash, string password);
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class HashingOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public HashingOptions() { }
        /// <summary>
        /// 
        /// </summary>
        public int Iterations { get; set; } = 10000;
    }
}
