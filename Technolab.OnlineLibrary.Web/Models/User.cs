using System.Security.Cryptography;
using System.Text;

namespace Technolab.OnlineLibrary.Web.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool VerifyPassword(string enteredPassword)
        {
            if (string.IsNullOrEmpty(enteredPassword))
                return false;

            string[] split = PasswordHash.Split(delimiter);

            string hashNoParams = split[0];
            byte[] salt = Convert.FromBase64String(split[1]);
            int iterations = Convert.ToInt32(split[2]);
            HashAlgorithmName hashAlgorithm = new HashAlgorithmName(split[3]);
            int keySize = Convert.ToInt32(split[4]);

            var enteredPasswordHash = Rfc2898DeriveBytes.Pbkdf2(enteredPassword, salt, iterations, hashAlgorithm, keySize);
            return CryptographicOperations.FixedTimeEquals(enteredPasswordHash, Convert.FromHexString(hashNoParams));
        }

        private string GenerateHash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(keySize);

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);

            string hashStr = Convert.ToHexString(hash) + delimiter +
                             Convert.ToBase64String(salt) + delimiter +
                             iterations + delimiter +
                             hashAlgorithm.Name + delimiter +
                             keySize;

            return hashStr;
        }

        private const char delimiter = ':';
        private const int iterations = 200000;
        private HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
        private const int keySize = 32;
    }
}