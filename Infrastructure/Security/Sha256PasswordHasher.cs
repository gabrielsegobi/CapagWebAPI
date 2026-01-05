using Infrastructure.Interface;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Security
{
    public class Sha256PasswordHasher : IPasswordHasher
    {
        public string Hash(string senha)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(senha);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool Verify(string senha, string hash)
        {
            var senhaHash = Hash(senha);
            return senhaHash == hash;
        }
    }
}
