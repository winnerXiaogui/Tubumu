using System;
using System.Security.Cryptography;
using System.Text;

namespace Tubumu.Modules.Framework.Utilities.Cryptography
{
    /// <summary>
    /// SHA1加密算法
    /// </summary>
    public static class SHA1
    {
        public static String Encrypt(String rawString)
        {
            if(rawString == null)
            {
                throw new ArgumentNullException(nameof(rawString));
            }

            return Convert.ToBase64String(EncryptToByteArray(rawString));
        }

        public static Byte[] EncryptToByteArray(String rawString)
        {
            if(rawString == null)
            {
                throw new ArgumentNullException(nameof(rawString));
            }

            Byte[] salted = Encoding.UTF8.GetBytes(rawString);
            System.Security.Cryptography.SHA1 hasher = new SHA1Managed();
            Byte[] hashed = hasher.ComputeHash(salted);
            return hashed;
        }
    }
}
