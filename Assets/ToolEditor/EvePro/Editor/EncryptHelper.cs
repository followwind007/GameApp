using System.Text;
using System.Security.Cryptography;

namespace ToolEditor.EvePro.Editor
{
    public class EncryptHelper
    {
        public static string SHAmd5Encrypt(string normalText)
        {
            var bytes = Encoding.Default.GetBytes(normalText);
            var Md5 = new MD5CryptoServiceProvider();
            var encryptbytes = Md5.ComputeHash(bytes);
            return Base64To16(encryptbytes);
        }

        private static string Base64To16(byte[] buffer)
        {
            string md_str = string.Empty;
            for (int i = 0; i < buffer.Length; i++)
            {
                md_str += buffer[i].ToString("x2");
            }
            return md_str;
        }

    }

}