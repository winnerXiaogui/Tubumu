using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Tubumu.Modules.Framework.Authorization
{
    public static class SignatureHelper
    {
        public static SigningCredentials GenerateSigningCredentials(string secretKey)
        {
            var signingKey = GenerateSigningKey(secretKey);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            return signingCredentials;
        }

        public static SymmetricSecurityKey GenerateSigningKey(string secretKey)
        {
            var keyByteArray = Encoding.UTF8.GetBytes(secretKey);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            return signingKey;
        }
    }
}
