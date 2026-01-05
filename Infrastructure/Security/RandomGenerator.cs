using System.Security.Cryptography;

namespace Infrastructure.Security
{
    public static class RandomGenerator
    {
        public static string GenerateDefault() => Generate(64);

        public static string Generate(int size = 64)
        {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }
    }
}
