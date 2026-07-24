using System;
using System.Security.Cryptography;
using System.Text;

namespace TechnicalTest.Helpers
{
    /// <summary>
    /// Hashing password: SHA256(password + salt), hasil hex UPPERCASE.
    /// Harus konsisten dengan hash yang dipakai di 05_InsertDummyData.sql.
    /// </summary>
    public static class PasswordHelper
    {
        public static string ComputeHash(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password + salt);
                var hashBytes = sha256.ComputeHash(bytes);

                var builder = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    builder.Append(b.ToString("X2"));
                }
                return builder.ToString();
            }
        }

        public static string GenerateSalt()
        {
            var bytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }
    }
}
