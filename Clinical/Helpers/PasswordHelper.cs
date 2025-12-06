using Clinical.Helpers;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Clinical.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string plain)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(plain);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash); // .NET 5+; or BitConverter if older
        }

        public static bool Verify(string plain, string hashed)
        {
            return HashPassword(plain) == hashed;
        }
    }
}
