using EasyReading.Application.Abstractions;
using System.Security.Cryptography;
using System.Text;

namespace EasyReading.Infrastructure.Services
{
    public class HashService : IHashService
    {
        public string GetHash(string value)
        {
            var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
