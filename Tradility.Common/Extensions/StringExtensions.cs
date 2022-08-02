using System;
using System.IO;
using System.Security.Cryptography;

namespace Tradility.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool Compare(this string a, string b) => a.Equals(b, StringComparison.OrdinalIgnoreCase);
        public static string ToMD5(this string a)
        {
            using var md5 = MD5.Create();
            var inputBytes = System.Text.Encoding.ASCII.GetBytes(a);
            var hashBytes = md5.ComputeHash(inputBytes);
            var md5Hash = Convert.ToHexString(hashBytes);
            return md5Hash;
        }
    }
}
