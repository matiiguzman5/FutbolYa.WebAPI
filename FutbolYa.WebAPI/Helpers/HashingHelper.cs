using System;
using System.Security.Cryptography;
using System.Text;

namespace FutbolYa.WebAPI.Helpers
{
    public static class HashingHelper
    {
        public static string ComputeSha256(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(value);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}
