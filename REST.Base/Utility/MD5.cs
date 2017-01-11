using System;
using System.Text;
using System.Security.Cryptography;
using System.ComponentModel;

namespace REST.Base.Utility
{
    /// <summary>
    /// MD5加密机密类
    /// </summary>
    [Description("MD5加密解密类")]
    public class MD5
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="sourceTxt">源字符串</param>
        /// <returns>返回密文</returns>
        public static string MD5Encrypt(string sourceTxt)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(sourceTxt));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// MD5验证
        /// </summary>
        /// <param name="sourceTxt">源字符串</param>
        /// <param name="encryptedTxt">密文</param>
        /// <returns>返回:True-验证成功,False-验证失败</returns>
        public static bool MD5Verify(string sourceTxt, string encryptedTxt)
        {
            string hashOfInput = MD5Encrypt(sourceTxt);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (0 == comparer.Compare(hashOfInput, encryptedTxt))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}