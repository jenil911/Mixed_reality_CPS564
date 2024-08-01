// WARNING: Auto generated code. Modifications will be lost!
using System.Security.Cryptography;
using System.Text;

namespace Unity.Services.Deployment.Editor.Shared.Crypto
{
    static class Hash
    {
        public static string SHA1(string input)
        {
            using var sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
