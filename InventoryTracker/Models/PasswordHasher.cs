using System.Security.Cryptography;
using System.Text;

namespace InventoryTracker.Models
{
    /// <summary>
    /// Provides methods for hashing passwords and verifying hashed passwords.
    /// </summary>
    public class PasswordHasher
    {
        /// <summary>
        /// Hashes a password using SHA-256 algorithm with a randomly generated salt.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="salt">The generated salt used in hashing.</param>
        /// <returns>The hashed password as a base64-encoded string.</returns>
        public string HashPassword(string password, out string salt)
        {
            byte[] saltBytes = GenerateSalt();
            salt = Convert.ToBase64String(saltBytes);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] combinedBytes = new byte[passwordBytes.Length + saltBytes.Length];
            Array.Copy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
            Array.Copy(saltBytes, 0, combinedBytes, passwordBytes.Length, saltBytes.Length);

            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }

        /// <summary>
        /// Generates a random salt using RNGCryptoServiceProvider.
        /// </summary>
        /// <returns>The generated salt as a byte array.</returns>
        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        /// <summary>
        /// Verifies if the entered password matches the stored hashed password.
        /// </summary>
        /// <param name="enteredPassword">The password entered by the user.</param>
        /// <param name="storedHash">The stored hashed password to compare against.</param>
        /// <param name="storedSalt">The stored salt used in hashing the password.</param>
        /// <returns>True if the entered password matches the stored hashed password; otherwise, false.</returns>
        public bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            byte[] hashedEnteredPassword = Convert.FromBase64String(storedHash);
            byte[] saltBytes = Convert.FromBase64String(storedSalt);

            byte[] enteredPasswordBytes = Encoding.UTF8.GetBytes(enteredPassword);
            byte[] combinedBytes = new byte[enteredPasswordBytes.Length + saltBytes.Length];
            Array.Copy(enteredPasswordBytes, 0, combinedBytes, 0, enteredPasswordBytes.Length);
            Array.Copy(saltBytes, 0, combinedBytes, enteredPasswordBytes.Length, saltBytes.Length);

            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
                return hashedEnteredPassword.SequenceEqual(hashedBytes);
            }
        }
    }
}
